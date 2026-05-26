namespace SmartInfluence.Data.Entities;

public class InfluencerScore
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public decimal EngagementScore { get; set; }
    public decimal BrandFitScore { get; set; }
    public decimal AvgViews { get; set; }
    public decimal AvgLikes { get; set; }
    public decimal AvgComments { get; set; }
    public DateTime CalculatedAt { get; set; }
    public Influencers? Influencer { get; set; }
}
