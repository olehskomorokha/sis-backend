namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    public sealed record UkrainianYouTubeBloggerDto
    {
        public string? Source { get; init; }
        public string? CountryCode { get; init; }
        public string? Language { get; init; }
        public required string ChannelId { get; init; }
        public required string ChannelUrl { get; init; }
        public required string Name { get; init; }
        public string? NickName { get; init; }
        public string? Description { get; init; }
        public string? AvatarUrl { get; init; }
        public DateTime IndexedAt { get; init; }
        public Fields Fields { get; init; }
        public Interests Interests { get; init; }
        public Statictics Statictics { get; init; }
    }

    public sealed record Fields
    {
        public DateTime? PublishedAt { get; init; }
        public int? Posts { get; init; }
        public int? SubscriberCount { get; init; }
        public int? ViewCount { get; init; }
        public Interests Interests { get; init; }
    }

    public sealed record Interests
    {
        public string[] ChannelTags { get; init; }
        public string[] VideoTags { get; init; }
    }
    public sealed record Statictics
    {
        
        public float EngagementRate { get; init; }
        public PerHalfYear  PerHalfYear { get; init; }
        
    }

    public sealed record PerHalfYear
    {
        public int VideoCount { get; init; }
        public float PostPerDay { get; init; }
        public int AvgLike { get; init; }
        public int AvgView { get; init; }
        public int AvgComment { get; init; }
        
        
    }
    
    public sealed record VideoDetailModel
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
