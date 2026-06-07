using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IClientInfluencerRepository : IRepository<ClientInfluencer>
{
    public Task<List<ClientInfluencer>> GetAllClientInfluencersAsync(int clientId);
    public Task UpdateAsync(ClientInfluencer entity);
    public Task<ClientInfluencer?> GetByClientAndInfluencerAsync(int clientId, int influencerId);
    public Task DeleteAsync(ClientInfluencer entity);
    public Task<bool> ExistsByInfluencerIdAsync(int influencerId);
}
