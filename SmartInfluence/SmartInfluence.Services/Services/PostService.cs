using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostResponseModel>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(PostMapper.MapToResponseModel).ToList();
    }

    public async Task<List<PostResponseModel>> GetByInfluencerIdAsync(int influencerId)
    {
        var posts = await _postRepository.GetByInfluencerIdAsync(influencerId);
        return posts.Select(PostMapper.MapToResponseModel).ToList();
    }
}
