namespace SmartInfluence.Services.Models;

public class InfluencerScoreModel
{
    public float EngagementRate { get; set; }
    public int PostCount { get; set; }
    public decimal AvgViews { get; set; }
    public decimal AvgLikes { get; set; }
    public decimal AvgComments { get; set; }
    public DateTime CalculatedAt { get; set; }
}