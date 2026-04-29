using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IClientRepository : IRepository
{
    public Task<Client?> GetByIdAsync(int id);
    
    public Task CreateAsync(Client client);
    
}