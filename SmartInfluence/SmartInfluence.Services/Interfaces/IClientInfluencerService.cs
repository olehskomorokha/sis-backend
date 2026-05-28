using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IClientInfluencerService
{
    public Task<List<ClientInfluencerModel>> GetAllAsync(int clientId);
}