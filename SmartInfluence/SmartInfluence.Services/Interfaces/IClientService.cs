using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IClientService
{
    public Task CreateAsync(CreateClientModel model);
}