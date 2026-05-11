using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IPostService
{
    public Task<List<PostResponseModel>> GetAllAsync();
    public Task<List<PostResponseModel>> GetByInfluencerIdAsync(int influencerId);
}
