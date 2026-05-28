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

    public Task<Influencers?> GetByInfluencerIdAsync(string influencerId)
    {
        return _dbContext.Influencers.FirstOrDefaultAsync(x => x.InfluencerId == influencerId);
    }

    public async Task<List<Influencers>> GetByClientIdAsync(int clientId)
    {
        return await _dbContext.ClientInfluencers
            .Where(x => x.ClientId == clientId && x.Influencer != null)
            .Select(x => x.Influencer!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<InfluencerScore>> GetLatestScoresByInfluencerIdsAsync(IEnumerable<int> influencerIds)
    {
        var ids = influencerIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        return await _dbContext.InfluencerScores
            .Where(x => ids.Contains(x.InfluencerId))
            .GroupBy(x => x.InfluencerId)
            .Select(group => group
                .OrderByDescending(x => x.CalculatedAt)
                .ThenByDescending(x => x.Id)
                .First())
            .ToListAsync();
    }

    public Task<bool> ClientInfluencerExistsAsync(int clientId, int influencerId)
    {
        return _dbContext.ClientInfluencers
            .AnyAsync(x => x.ClientId == clientId && x.InfluencerId == influencerId);
    }

    public async Task CreateAsync(Influencers influencer)
    {
        _dbContext.Influencers.Add(influencer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Influencers influencer)
    {
        _dbContext.Influencers.Update(influencer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddScoreAsync(InfluencerScore score)
    {
        _dbContext.InfluencerScores.Add(score);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddClientInfluencerAsync(ClientInfluencer model)
    {
        _dbContext.ClientInfluencers.Add(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteScoresByInfluencerIdAsync(int influencerId)
    {
        var scores = await _dbContext.InfluencerScores
            .Where(x => x.InfluencerId == influencerId)
            .ToListAsync();

        if (scores.Count == 0)
        {
            return;
        }

        _dbContext.InfluencerScores.RemoveRange(scores);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Influencers influencer)
    {
        _dbContext.Influencers.Remove(influencer);
        await _dbContext.SaveChangesAsync();
    }
}
