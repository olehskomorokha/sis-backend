using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _dbContext;

    public PostRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Post>> GetAllAsync()
    {
        return await _dbContext.Posts.ToListAsync();
    }

    public async Task<List<Post>> GetByInfluencerIdAsync(int influencerId)
    {
        return await _dbContext.Posts
            .Where(x => x.InfluencerId == influencerId)
            .ToListAsync();
    }
}
