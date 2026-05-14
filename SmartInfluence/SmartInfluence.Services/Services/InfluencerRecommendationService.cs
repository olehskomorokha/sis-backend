using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;
using SmartInfluence.Data.Models;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class InfluencerRecommendationService : IInfluencerRecommendationService
{
    private readonly IInfluencerRecommendationRepository _recommendationRepository;
    private readonly IProductQueryAiService _productQueryAiService;

    public InfluencerRecommendationService(
        IInfluencerRecommendationRepository recommendationRepository,
        IProductQueryAiService productQueryAiService)
    {
        _recommendationRepository = recommendationRepository;
        _productQueryAiService = productQueryAiService;
    }

    public async Task<InfluencerRecommendationResponseModel> RecommendAsync(InfluencerRecommendationRequestModel request)
    {
        var criteria = await _productQueryAiService.ParseProductDescriptionAsync(request.ProductDescription);
        var candidates = await _recommendationRepository.GetCandidatesAsync();

        var results = candidates
            .Where(candidate => MatchesClientFilters(candidate, request))
            .Select(candidate => BuildRecommendation(candidate, criteria))
            .Where(result => result != null)
            .Select(result => result!)
            .OrderByDescending(result => result.Score)
            .Take(Math.Clamp(request.Limit, 1, 50))
            .ToList();

        return new InfluencerRecommendationResponseModel
        {
            Criteria = criteria,
            Influencers = results
        };
    }

    private static bool MatchesClientFilters(
        InfluencerRecommendationCandidateData candidate,
        InfluencerRecommendationRequestModel request)
    {
        var filters = request.Filters;

        if (!string.IsNullOrWhiteSpace(filters.Platform) &&
            !string.Equals(candidate.Influencer.Platform, filters.Platform, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(filters.Country) &&
            !string.Equals(candidate.Influencer.Country, filters.Country, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (filters.Tags.Count > 0 && !MatchesTags(candidate, filters.Tags))
        {
            return false;
        }

        if (filters.MinFollowersCount.HasValue &&
            candidate.Influencer.FollowersCount < filters.MinFollowersCount.Value)
        {
            return false;
        }

        if (filters.MaxFollowersCount.HasValue &&
            candidate.Influencer.FollowersCount > filters.MaxFollowersCount.Value)
        {
            return false;
        }

        if (filters.MinEngagementRatePercent.HasValue)
        {
            var engagementRatePercent = CalculateEngagementRate(candidate.Influencer) * 100m;
            if (engagementRatePercent < filters.MinEngagementRatePercent.Value)
            {
                return false;
            }
        }

        return true;
    }

    private RecommendedInfluencerModel? BuildRecommendation(
        InfluencerRecommendationCandidateData candidate,
        ProductCriteriaModel criteria)
    {
        var authenticityGate = criteria.MaxFakeFollowersPercent;
        if (authenticityGate.HasValue &&
            authenticityGate.Value > 0 &&
            candidate.Audience != null &&
            candidate.Audience.FakeFollowersPercent > authenticityGate.Value)
        {
            return null;
        }

        var platformScore = CalculatePlatformScore(candidate.Influencer.Platform, criteria.Platforms);
        if (criteria.Platforms.Count > 0 && platformScore == 0)
        {
            return null;
        }

        var topicScore = CalculateTopicScore(candidate, criteria);
        var audienceScore = CalculateAudienceScore(candidate.Audience, criteria);
        var engagementScore = CalculateEngagementScore(candidate.Influencer);
        var authenticityScore = CalculateAuthenticityScore(candidate.Audience, candidate.LatestScore);
        var brandFit = (topicScore * 0.7m) + (platformScore * 0.3m);

        var finalScore = (brandFit * 0.4m) +
                         (audienceScore * 0.3m) +
                         (engagementScore * 0.2m) +
                         (authenticityScore * 0.1m);

        if (finalScore <= 0.15m)
        {
            return null;
        }

        return new RecommendedInfluencerModel
        {
            InfluencerId = candidate.Influencer.Id,
            UserName = candidate.Influencer.UserName,
            FullName = candidate.Influencer.FullName,
            Platform = candidate.Influencer.Platform,
            Country = candidate.Influencer.Country,
            Score = Math.Round(finalScore, 4),
            Reason = BuildReason(candidate, criteria, topicScore, audienceScore, authenticityScore)
        };
    }

    private static decimal CalculatePlatformScore(string platform, IReadOnlyCollection<string> requestedPlatforms)
    {
        if (requestedPlatforms.Count == 0)
        {
            return 1m;
        }

        return requestedPlatforms.Any(x => string.Equals(x, platform, StringComparison.OrdinalIgnoreCase)) ? 1m : 0m;
    }

    private static decimal CalculateTopicScore(
        InfluencerRecommendationCandidateData candidate,
        ProductCriteriaModel criteria)
    {
        if (criteria.Topics.Count == 0 && string.IsNullOrWhiteSpace(criteria.ProductCategory))
        {
            return 0.6m;
        }

        var influencerTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var term in SplitTerms(candidate.Influencer.Bio))
        {
            influencerTerms.Add(term);
        }

        foreach (var tag in candidate.Tags.SelectMany(SplitTerms))
        {
            influencerTerms.Add(tag);
        }

        foreach (var topic in candidate.Topics)
        {
            influencerTerms.Add(topic);
        }

        var requiredTerms = new HashSet<string>(criteria.Topics, StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(criteria.ProductCategory))
        {
            requiredTerms.Add(criteria.ProductCategory);
        }

        if (requiredTerms.Count == 0)
        {
            return 0.6m;
        }

        var matches = requiredTerms.Count(term => influencerTerms.Contains(term));
        return (decimal)matches / requiredTerms.Count;
    }

    private static decimal CalculateAudienceScore(Audience? audience, ProductCriteriaModel criteria)
    {
        if (audience == null)
        {
            return 0.35m;
        }

        var scores = new List<decimal>();

        if (!string.IsNullOrWhiteSpace(criteria.TargetCountry))
        {
            var countryMatch = audience.TopCountries.Contains(criteria.TargetCountry, StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(criteria.TargetCountry, audience.Influencer?.Country, StringComparison.OrdinalIgnoreCase);
            scores.Add(countryMatch ? 1m : 0m);
        }

        if (!string.IsNullOrWhiteSpace(criteria.TargetGender))
        {
            scores.Add(criteria.TargetGender.Equals("female", StringComparison.OrdinalIgnoreCase)
                ? NormalizePercent(audience.FemalePercent)
                : NormalizePercent(audience.MalePercent));
        }

        var hasAgeCriteria = criteria.AgeMin.GetValueOrDefault() > 0 || criteria.AgeMax.GetValueOrDefault() > 0;
        if (hasAgeCriteria)
        {
            scores.Add(CalculateAgeScore(audience, criteria.AgeMin, criteria.AgeMax));
        }

        return scores.Count == 0 ? 0.6m : scores.Average();
    }

    private static decimal CalculateEngagementScore(Influencers influencer)
    {
        var engagementRate = CalculateEngagementRate(influencer);
        return Math.Clamp(engagementRate / 0.15m, 0m, 1m);
    }

    private static decimal CalculateEngagementRate(Influencers influencer)
    {
        if (influencer.FollowersCount <= 0)
        {
            return 0m;
        }

        var weightedEngagement = influencer.AvgLikes + influencer.AvgComments * 2m;
        return weightedEngagement / influencer.FollowersCount;
    }

    private static decimal CalculateAuthenticityScore(Audience? audience, InfluencerScore? score)
    {
        var values = new List<decimal>();

        if (audience != null)
        {
            values.Add(1m - Math.Clamp(audience.FakeFollowersPercent / 100m, 0m, 1m));
        }

        if (score != null)
        {
            values.Add(NormalizeScore(score.ScoreAuthenticity));
        }

        return values.Count == 0 ? 0.5m : values.Average();
    }

    private static string BuildReason(
        InfluencerRecommendationCandidateData candidate,
        ProductCriteriaModel criteria,
        decimal topicScore,
        decimal audienceScore,
        decimal authenticityScore)
    {
        var reasons = new List<string>();

        if (topicScore >= 0.5m)
        {
            reasons.Add("content topics match the product niche");
        }

        if (audienceScore >= 0.5m)
        {
            reasons.Add("audience profile is close to the target segment");
        }

        if (authenticityScore >= 0.6m && candidate.Audience != null)
        {
            reasons.Add($"fake followers {candidate.Audience.FakeFollowersPercent:0.##}%");
        }

        if (criteria.Platforms.Count > 0 &&
            criteria.Platforms.Any(x => string.Equals(x, candidate.Influencer.Platform, StringComparison.OrdinalIgnoreCase)))
        {
            reasons.Add($"works on {candidate.Influencer.Platform}");
        }

        return reasons.Count == 0 ? "matched by aggregated ranking signals" : string.Join("; ", reasons);
    }

    private static decimal CalculateAgeScore(Audience audience, int? ageMin, int? ageMax)
    {
        var targetMin = ageMin ?? 18;
        var targetMax = ageMax ?? 44;

        var buckets = new List<(int Min, int Max, decimal Value)>
        {
            (18, 24, NormalizePercent(audience.Age18_24)),
            (25, 34, NormalizePercent(audience.Age25_34)),
            (35, 44, NormalizePercent(audience.Age35_44))
        };

        var matched = buckets
            .Where(bucket => bucket.Min <= targetMax && bucket.Max >= targetMin)
            .Select(bucket => bucket.Value)
            .ToList();

        return matched.Count == 0 ? 0m : matched.Average();
    }

    private static decimal NormalizePercent(decimal value)
    {
        return Math.Clamp(value / 100m, 0m, 1m);
    }

    private static decimal NormalizeScore(decimal value)
    {
        return value > 1m ? Math.Clamp(value / 100m, 0m, 1m) : Math.Clamp(value, 0m, 1m);
    }

    private static bool MatchesTags(
        InfluencerRecommendationCandidateData candidate,
        IReadOnlyCollection<string> requestedTags)
    {
        var candidateTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var term in SplitTerms(candidate.Influencer.Bio))
        {
            candidateTerms.Add(term);
        }

        foreach (var tag in candidate.Tags.SelectMany(SplitTerms))
        {
            candidateTerms.Add(tag);
        }

        foreach (var topic in candidate.Topics.SelectMany(SplitTerms))
        {
            candidateTerms.Add(topic);
        }

        foreach (var requestedTag in requestedTags)
        {
            foreach (var term in SplitTerms(requestedTag))
            {
                if (candidateTerms.Contains(term))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IEnumerable<string> SplitTerms(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? []
            : text
                .Split([',', ';', '.', '#', ' ', '\n', '\r', '\t', '/', '\\', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToLowerInvariant());
    }
}
