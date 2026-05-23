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

    public async Task<string> AiExplanation(InfluencerResponseModel model)
    {

        return "";
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
            authenticityGate.Value > 0)
        {
            return null;
        }

        var platformScore = CalculatePlatformScore(candidate.Influencer.Platform, criteria.Platforms);
        if (criteria.Platforms.Count > 0 && platformScore == 0)
        {
            return null;
        }

        var topicScore = CalculateTopicScore(candidate, criteria);
        var engagementScore = CalculateEngagementScore(candidate.Influencer);
        var brandFit = (topicScore * 0.7m) + (platformScore * 0.3m);

        var finalScore = (brandFit * 0.4m) +
                         (engagementScore * 0.2m);

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
            Reason = BuildReason(candidate, criteria, topicScore)
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

    private static string BuildReason(
        InfluencerRecommendationCandidateData candidate,
        ProductCriteriaModel criteria,
        decimal topicScore)
    {
        var reasons = new List<string>();

        if (topicScore >= 0.5m)
        {
            reasons.Add("content topics match the product niche");
        }
        
        if (criteria.Platforms.Count > 0 &&
            criteria.Platforms.Any(x => string.Equals(x, candidate.Influencer.Platform, StringComparison.OrdinalIgnoreCase)))
        {
            reasons.Add($"works on {candidate.Influencer.Platform}");
        }

        return reasons.Count == 0 ? "matched by aggregated ranking signals" : string.Join("; ", reasons);
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
