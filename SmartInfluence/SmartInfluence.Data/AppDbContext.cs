using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientInfluencer> ClientInfluencers => Set<ClientInfluencer>();
    public DbSet<Influencers> Influencers => Set<Influencers>();
    public DbSet<InfluencerScore> InfluencerScores => Set<InfluencerScore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }
        }

        modelBuilder.Entity<Influencers>()
            .HasIndex(x => x.InfluencerId)
            .IsUnique();

        modelBuilder.Entity<ClientInfluencer>()
            .HasIndex(x => new { x.ClientId, x.InfluencerId })
            .IsUnique();
    }
}
