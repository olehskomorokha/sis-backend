using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class InfluencerRepository : IInfluencerRepository
{
    private readonly AppDbContext _dbContext;

    public InfluencerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Influencers>> GetAllAsync()
    {
        return await _dbContext.Influencers.ToListAsync();
    }

    public Task<Influencers?> GetByIdAsync(int id)
    {
        return _dbContext.Influencers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Influencers>> GetByClientIdAsync(int clientId)
    {
        return _dbContext.ClientInfluencers
            .Where(x => x.CampaignId == clientId && x.Influencer != null)
            .Select(x => x.Influencer!)
            .Distinct()
            .ToListAsync();
    }
}
