using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IInfluencerRepository : IRepository<Influencers>
{
    public Task<Influencers?> GetByIdAsync(int id);
    public Task<List<Influencers>> GetByClientIdAsync(int clientId);
}
