using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IInfluencerRepository : IRepository<Influencers>
{
    public Task<Influencers?> GetByIdAsync(int id);
    public Task<Influencers?> GetByInfluencerIdAsync(string influencerId);
    public Task<List<Influencers>> GetByClientIdAsync(int clientId);
    public Task<bool> ClientInfluencerExistsAsync(int clientId, int influencerId);
    public Task CreateAsync(Influencers influencer);
    public Task AddClientInfluencerAsync(ClientInfluencer model);
    public Task UpdateAsync(Influencers influencer);
    public Task AddScoreAsync(InfluencerScore score);
}
