using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class ClientInfluencerRepository :  IClientInfluencerRepository
{
    private readonly AppDbContext _dbContext;

    public ClientInfluencerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ClientInfluencer>> GetAllAsync(int clientId)
    {
        return await _dbContext.ClientInfluencers
            .Where(x => x.ClientId == clientId)
            .Include(x => x.Influencer)
            .ToListAsync();
    }
    public async Task UpdateAsync(ClientInfluencer entity)
    {
        _dbContext.ClientInfluencers.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
