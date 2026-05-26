namespace SmartInfluence.Services.Models;

public class RecommendedChannelModel
{
    public double? score { get; set; }
    public string ChannelName { get; set; }
    public string? ChannelId { get; set; }
    public string ChannelUrl { get; set; }
    public string CountryCode { get; set; }
    public string Description { get; set; }
    public string AvatarUrl { get; set; }
    public float EngagementRate { get; set; }
    public int VideoCount { get; set; }
    public float PostPerDay { get; set; }
    public int AvgLike { get; set; }
    public int AvgView { get; set; }
    public int AvgComment { get; set; }
    public string? AiReview { get; set; }
}
