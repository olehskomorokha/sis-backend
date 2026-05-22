namespace SmartInfluence.Services.Models;

public class InfluencerSearchModel
{
    public string ChannelId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string PublishedAt { get; set; }
    public int? VideoCount { get; set; }
    public int? SubscriptionCount { get; set; }
    public int? ViewCount { get; set; }
    public string channelUrl { get; set; }
    public string Tags { get; set; }
    public InfluencerStatistics? Statistics { get; set; }
}