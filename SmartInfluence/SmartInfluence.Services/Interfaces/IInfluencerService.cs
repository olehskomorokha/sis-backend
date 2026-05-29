using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IInfluencerService
{
    public Task<List<InfluencerResponseModel>> GetAllAsync();
    public Task<List<InfluencerResponseModel>> GetByClientIdAsync(int clientId);
    public Task<InfluencerResponseModel?> GetByIdAsync(int id);
    public Task<InfluencerResponseModel> SaveRecommendedAsync(RecommendedChannelModel model, int clientId);
    Task<List<RecommendedChannelModel>> RecommendAsync(
        InfluencerRecommendationFiltersModel filters);
}
