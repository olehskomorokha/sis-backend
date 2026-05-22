namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    public sealed record UkrainianYouTubeBloggerDto
    {
        public required string ChannelId { get; init; }

        public required string Title { get; init; }

        public string? Description { get; init; }

        public string? CustomUrl { get; init; }

        public string? PublishedAt { get; init; }

        public string? Country { get; init; }

        public ulong? VideoCount { get; init; }

        public ulong? SubscriberCount { get; init; }

        public ulong? ViewCount { get; init; }

        public string? ThumbnailUrl { get; init; }
        public DateTime IndexedAt { get; init; }

        public required string ChannelUrl { get; init; }
        public required string Uploads  { get; init; }
        public required Statictics Statictics { get; init; }
    }

    public sealed record Statictics
    {
        public int ViewCount { get; init; }
        public ulong? SubscriberCount { get; init; }
        public bool HiddenSubscriberCount { get; init; }
        public ulong? VideoCount { get; init; }
    }

    public sealed record PlayListItems
    {
        public string? VideoId { get; init; }
        public string? PublishedAt { get; init; }
        public string? PlaylistId { get; init; }
        public string? ChannelId { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? ChannelTitle { get; init; }
    }

    public sealed record VideoDetails
    {
        public required string VideoId { get; init; }

        public string? ChannelId { get; init; }

        public string? ChannelTitle { get; init; }

        public string? Title { get; init; }

        public string? Description { get; init; }

        public string? PublishedAt { get; init; }

        public string? ThumbnailUrl { get; init; }

        public string? Tags { get; init; }
        
        public string? TopicCategories { get; init; }

        public string? CategoryId { get; init; }

        public string? DefaultLanguage { get; init; }

        public string? Duration { get; init; }
        
        public string? Definition { get; init; }

        public string? Caption { get; init; }

        public bool LicensedContent { get; init; }

        public string? UploadStatus { get; init; }

        public ulong? ViewCount { get; init; }

        public ulong? LikeCount { get; init; }

        public ulong? FavoriteCount { get; init; }

        public ulong? CommentCount { get; init; }
    }
}
