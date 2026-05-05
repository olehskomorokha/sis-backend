using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IClientService
{
    public Task<List<ClientResponseModel>> GetAllAsync();
    public Task CreateAsync(CreateClientModel model);
    public Task<string> LoginAsync(LoginClientModel model);
}
