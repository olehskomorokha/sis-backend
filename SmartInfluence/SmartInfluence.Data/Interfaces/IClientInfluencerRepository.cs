using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IClientInfluencerRepository
{
    public Task<List<ClientInfluencer>> GetAllAsync(int clientId);
    public Task UpdateAsync(ClientInfluencer entity);
}