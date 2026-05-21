using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Models;

public class InfluencerRecommendationCandidateData
{
    public required Influencers Influencer { get; init; }
    public Audience? Audience { get; init; }
    public InfluencerScore? LatestScore { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Topics { get; init; } = [];
}
