using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public class ElasticsearchMapper
{
    public static InfluencerSearchModel MapToInfluencerSearchModel(BloggerDocument document)
    {
        return new InfluencerSearchModel
        {
            ChannelId = document.ChannelId ?? string.Empty,
            Title = document.Title ?? string.Empty,
            Description = document.Description ?? string.Empty,
            PublishedAt = document.PublishedAt ?? string.Empty,
            VideoCount = document.VideoCount,
            SubscriptionCount = document.SubscriberCount,
            ViewCount = document.ViewCount ?? 0,
            channelUrl = document.ChannelUrl ?? string.Empty,
            Tags = document.TopicCategories ?? string.Empty,
        };
    }

}

public class BloggerDocument
{
    public string? ChannelId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PublishedAt { get; set; }
    public int? VideoCount { get; set; }
    public int? SubscriberCount { get; set; }
    public int? ViewCount { get; set; }
    public string? ChannelUrl { get; set; }
    public string? TopicCategories { get; set; }
}
public class Statistics
{
    public int? ViewCount { get; set; }
    public int? SubscriberCount { get; set; }
    public int HiddenSubscriberCount { get; set; }
    public long? VideoCount { get; set; }
}