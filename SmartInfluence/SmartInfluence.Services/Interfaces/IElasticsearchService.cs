using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IElasticsearchService
{
    public Task<List<string>> GetAllBloggerTags();
    public Task<List<InfluencerSearchModel>> GetByFilters(FiltersModel model);
    public Task<List<VideoDetailModel>> GetAllVideoDetailsByChannel(string channelId);
    public Task<object> GetById(string id);
}