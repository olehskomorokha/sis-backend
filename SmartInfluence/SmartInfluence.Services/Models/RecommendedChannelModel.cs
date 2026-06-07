namespace SmartInfluence.Services.Models;

public class RecommendedChannelModel
{
    public double? Score { get; set; }
    public decimal BrandFitScore { get; set; }
    public decimal EngagementScore { get; set; }
    public decimal PostFrequencyScore { get; set; }
    public decimal PredictedEngagement { get; set; }
    public int TotalScore { get; set; }
    public string ChannelName { get; set; }
    public string? ChannelId { get; set; }
    public string ChannelUrl { get; set; }
    public string CountryCode { get; set; }
    public string Language { get; set; }
    public string Description { get; set; }
    public string AvatarUrl { get; set; }
    public int FollowersCount { get; set; }
    public float EngagementRate { get; set; }
    public int VideoCount { get; set; }
    public float PostPerDay { get; set; }
    public int AvgLike { get; set; }
    public int AvgView { get; set; }
    public int AvgComment { get; set; }
    public string? AiReview { get; set; }
}
