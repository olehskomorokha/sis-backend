using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IClientService
{
    public Task<List<ClientResponseModel>> GetAllAsync();
    public Task<ClientResponseModel> GetByIdAsync(int id);
    public Task CreateAsync(CreateClientModel model);
    public Task<bool> UpdateAsync(int id, UpdateClientModel model);
    public Task<string> LoginAsync(LoginClientModel model);
    public Task<List<InfluencerResponseModel>> GetInfluencersByClientIdAsync(int clientId);
    public Task DeleteAsync(int id);
}
