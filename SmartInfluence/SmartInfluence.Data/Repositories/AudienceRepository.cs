using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class AudienceRepository : IAudienceRepository
{
    private readonly AppDbContext _dbContext;

    public AudienceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Audience?> GetByInfluencerIdAsync(int influencerId)
    {
        return _dbContext.Audiences.FirstOrDefaultAsync(x => x.InfluencerId == influencerId);
    }
}
