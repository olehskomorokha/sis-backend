using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    public Task<Client?> GetByIdAsync(int id);
    public Task<Client?> GetByEmailAsync(string email);
    public Task CreateAsync(Client client);
}
