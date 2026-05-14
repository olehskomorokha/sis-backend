using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IInfluencerRecommendationService
{
    Task<InfluencerRecommendationResponseModel> RecommendAsync(InfluencerRecommendationRequestModel request);
}
