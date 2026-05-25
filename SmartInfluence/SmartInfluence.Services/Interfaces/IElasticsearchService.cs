using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IElasticsearchService
{
    public Task<List<string>> GetAllBloggerTags();
    public Task<List<object>> GetByFilters(FiltersModel model);
    public Task<object> GetById(string id);
}