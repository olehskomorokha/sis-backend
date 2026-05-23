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
            SubscriberCount = document.SubscriberCount,
            ViewCount = document.ViewCount ?? 0,
            channelUrl = document.ChannelUrl ?? string.Empty,
            Tags = document.TopicCategories ?? string.Empty,
        };
    }

    public static VideoDetailModel MapToVideoDetailModel(VideoDetailDocument document)
    {
        return new VideoDetailModel
        {
            VideoId = document.VideoId ?? string.Empty,
            ChannelId = document.ChannelId ?? string.Empty,
            ChannelTitle = document.ChannelTitle ?? string.Empty,
            Title = document.Title ?? string.Empty,
            Description = document.Description ?? string.Empty,
            PublishedAt = document.PublishedAt ?? string.Empty,
            ThumbnailUrl = document.ThumbnailUrl ?? string.Empty,
            Tags = document.Tags ?? string.Empty,
            TopicCategories = document.TopicCategories ?? string.Empty,
            CategoryId = document.CategoryId ?? string.Empty,
            DefaultLanguage = document.DefaultLanguage ?? string.Empty,
            Duration = document.Duration ?? string.Empty,
            Definition = document.Definition ?? string.Empty,
            Caption = document.Caption ?? string.Empty,
            LicensedContent = document.LicensedContent,
            UploadStatus = document.UploadStatus ?? string.Empty,
            ViewCount = document.ViewCount,
            LikeCount = document.LikeCount,
            FavoriteCount = document.FavoriteCount,
            CommentCount = document.CommentCount
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

public class VideoDetailDocument
{
    public string? VideoId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelTitle { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PublishedAt { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Tags { get; set; }
    public string? TopicCategories { get; set; }
    public string? CategoryId { get; set; }
    public string? DefaultLanguage { get; set; }
    public string? Duration { get; set; }
    public string? Definition { get; set; }
    public string? Caption { get; set; }
    public bool LicensedContent { get; set; }
    public string? UploadStatus { get; set; }
    public ulong? ViewCount { get; set; }
    public ulong? LikeCount { get; set; }
    public ulong? FavoriteCount { get; set; }
    public ulong? CommentCount { get; set; }
}
