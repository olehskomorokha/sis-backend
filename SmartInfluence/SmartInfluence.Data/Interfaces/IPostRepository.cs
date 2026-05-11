using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    public Task<List<Post>> GetByInfluencerIdAsync(int influencerId);
}
