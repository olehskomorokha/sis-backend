using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Client>> GetAllAsync()
    {
        return await _dbContext.Clients.ToListAsync();
    }

    public Task<Client?> GetByIdAsync(int id)
    {
        return Task.FromResult(_dbContext.Clients.FirstOrDefault(x => x.Id == id));
    }

    public Task<Client?> GetByEmailAsync(string email)
    {
        return _dbContext.Clients.FirstOrDefaultAsync(x => x.Email == email);
    }

    public Task CreateAsync(Client client)
    {
        _dbContext.Clients.Add(client);
        return _dbContext.SaveChangesAsync();
    }
}
