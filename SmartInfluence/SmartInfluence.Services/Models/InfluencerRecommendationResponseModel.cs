namespace SmartInfluence.Services.Models;

public class InfluencerRecommendationResponseModel
{
    public required ProductCriteriaModel Criteria { get; init; }
    public required List<RecommendedInfluencerModel> Influencers { get; init; }
}
