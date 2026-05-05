using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Audience> Audiences => Set<Audience>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientInfluencer> ClientInfluencers => Set<ClientInfluencer>();
    public DbSet<ContentAnalysis> ContentAnalyses => Set<ContentAnalysis>();
    public DbSet<FraudSignal> FraudSignals => Set<FraudSignal>();
    public DbSet<Influencers> Influencers => Set<Influencers>();
    public DbSet<InfluencerScore> InfluencerScores => Set<InfluencerScore>();
    public DbSet<InfluencerStatsHistory> InfluencerStatsHistories => Set<InfluencerStatsHistory>();
    public DbSet<InfluencerTag> InfluencerTags => Set<InfluencerTag>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InfluencerTag>()
            .HasKey(influencerTag => new { influencerTag.InfluencerId, influencerTag.TagId });

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
    }
}
