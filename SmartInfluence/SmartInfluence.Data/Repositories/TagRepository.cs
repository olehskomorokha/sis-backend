using Microsoft.EntityFrameworkCore;
using SmartInfluence.Data.Entities;
using SmartInfluence.Data.Interfaces;

namespace SmartInfluence.Data.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _dbContext.Tags.ToListAsync();
    }
}
