namespace SmartInfluence.Services.Models;

public class ElasticInfluencerRecommendationResponseModel
{
    public required ProductCriteriaModel Criteria { get; init; }
    public required List<RecommendedChannelModel> Channels { get; init; }
}
