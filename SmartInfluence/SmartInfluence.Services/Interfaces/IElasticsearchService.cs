using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IElasticsearchService
{
    public Task<List<string>> GetAllBloggerTags();
    public Task<List<RecommendedChannelModel>> RecommendBloggersAsync(ProductCriteriaModel criteria, InfluencerRecommendationFiltersModel filters);
    public Task<object> GetById(string id);
}
