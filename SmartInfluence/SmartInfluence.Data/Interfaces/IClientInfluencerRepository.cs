using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IClientInfluencerRepository
{
    public Task<List<ClientInfluencer>> GetAllAsync(int clientId);
    public Task UpdateAsync(ClientInfluencer entity);
    public Task<ClientInfluencer?> GetByClientAndInfluencerAsync(int clientId, int influencerId);
    public Task DeleteAsync(ClientInfluencer entity);
    public Task<bool> ExistsByInfluencerIdAsync(int influencerId);
}
