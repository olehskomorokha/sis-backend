using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Interfaces;
using SmartInfluence.Data.Models;

namespace SmartInfluence.Data.Repositories;

public class InfluencerRecommendationRepository : IInfluencerRecommendationRepository
{
    private readonly AppDbContext _dbContext;

    public InfluencerRecommendationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InfluencerRecommendationCandidateData>> GetCandidatesAsync()
    {
        var influencers = await _dbContext.Influencers.AsNoTracking().ToListAsync();
        var latestScores = await _dbContext.InfluencerScores.AsNoTracking()
            .GroupBy(x => x.InfluencerId)
            .Select(group => group.OrderByDescending(x => x.CalculatedAt).First())
            .ToDictionaryAsync(x => x.InfluencerId);

        var tags = await (
            from influencerTag in _dbContext.InfluencerTags.AsNoTracking()
            join tag in _dbContext.Tags.AsNoTracking() on influencerTag.TagId equals tag.Id
            select new { influencerTag.InfluencerId, tag.Name }
        ).ToListAsync();

        var topics = await (
            from post in _dbContext.Posts.AsNoTracking()
            join analysis in _dbContext.ContentAnalyses.AsNoTracking() on post.Id equals analysis.PostId
            select new { post.InfluencerId, analysis.Topics, post.Caption, post.Hashtags }
        ).ToListAsync();

        var tagLookup = tags
            .GroupBy(x => x.InfluencerId)
            .ToDictionary(group => group.Key, group => group.Select(x => x.Name).ToList());

        var topicLookup = topics
            .GroupBy(x => x.InfluencerId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .SelectMany(x => SplitTerms($"{x.Topics},{x.Caption},{x.Hashtags}"))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList());

        return influencers.Select(influencer => new InfluencerRecommendationCandidateData
        {
            Influencer = influencer,
            LatestScore = latestScores.GetValueOrDefault(influencer.Id),
            Tags = tagLookup.GetValueOrDefault(influencer.Id, []),
            Topics = topicLookup.GetValueOrDefault(influencer.Id, [])
        }).ToList();
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
