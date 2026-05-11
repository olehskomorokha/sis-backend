using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface ITagService
{
    public Task<List<TagResponseModel>> GetAllAsync();
}
