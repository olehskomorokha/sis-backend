using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IInfluencerRecommendationService
{
    Task<ElasticInfluencerRecommendationResponseModel> RecommendAsync(
        InfluencerRecommendationFiltersModel filters);
}
