namespace SmartInfluence.Data.Entities;

public class InfluencerScore
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public decimal EngagementScore { get; set; }
    public decimal BrandFitScore { get; set; }
    public DateTime CalculatedAt { get; set; }

    public Influencers? Influencer { get; set; }
}
