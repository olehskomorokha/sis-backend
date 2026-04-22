namespace SmartInfluence.Data.Entities;

public class FraudSignal
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public decimal SuspiciousGrowthScore { get; set; }
    public decimal BotActivityScore { get; set; }
    public decimal EngagementMismatchScore { get; set; }

    public Influencers? Influencer { get; set; }
}
