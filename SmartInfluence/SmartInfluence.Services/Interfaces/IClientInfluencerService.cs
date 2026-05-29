using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IClientInfluencerService
{
    public Task<List<ClientInfluencerModel>> GetAllAsync(int clientId);
    public Task<bool> UpdateAsync(int clientId, int influencerId, UpdateClientInfluencerModel model);
    public Task<bool> DeleteAsync(int clientId, int influencerId);
}
