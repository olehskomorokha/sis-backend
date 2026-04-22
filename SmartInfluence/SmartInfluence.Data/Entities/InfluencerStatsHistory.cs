namespace SmartInfluence.Data.Entities;

public class InfluencerStatsHistory
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public int FollowersCount { get; set; }
    public decimal AvgLikes { get; set; }
    public decimal EngagementRate { get; set; }
    public DateTime Date { get; set; }

    public Influencers? Influencer { get; set; }
}
