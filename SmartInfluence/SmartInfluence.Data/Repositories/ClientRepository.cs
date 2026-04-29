using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _dbContext;

    public ClientRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task GetAllAsync()
    {
        return Task.FromResult(_dbContext.Clients.ToList());
    }

    public Task<Client?> GetByIdAsync(int id)
    {
        return Task.FromResult(_dbContext.Clients.FirstOrDefault(x => x.Id == id));
    }

    public Task CreateAsync(Client client)
    {
        _dbContext.Clients.Add(client);
        return _dbContext.SaveChangesAsync();
    }
}