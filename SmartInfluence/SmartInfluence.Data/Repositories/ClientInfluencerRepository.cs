using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class ClientInfluencerRepository : IClientInfluencerRepository
{
    private readonly AppDbContext _dbContext;

    public ClientInfluencerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ClientInfluencer>> GetAllAsync()
    {
        return await  _dbContext.ClientInfluencers.AsNoTracking().ToListAsync();
    }
    
    public async Task<List<ClientInfluencer>> GetAllClientInfluencersAsync(int clientId)
    {
        return await _dbContext.ClientInfluencers
            .Where(x => x.ClientId == clientId)
            .Include(x => x.Influencer)
            .ToListAsync();
    }

    public async Task<ClientInfluencer?> GetByClientAndInfluencerAsync(int clientId, int influencerId)
    {
        return await _dbContext.ClientInfluencers
            .FirstOrDefaultAsync(x => x.ClientId == clientId && x.InfluencerId == influencerId);
    }

    public Task<bool> ExistsByInfluencerIdAsync(int influencerId)
    {
        return _dbContext.ClientInfluencers.AnyAsync(x => x.InfluencerId == influencerId);
    }

    public async Task UpdateAsync(ClientInfluencer entity)
    {
        _dbContext.ClientInfluencers.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ClientInfluencer entity)
    {
        _dbContext.ClientInfluencers.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
