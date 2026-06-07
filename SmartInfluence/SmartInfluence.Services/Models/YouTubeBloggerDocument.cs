namespace SmartInfluence.Services.Models;

public sealed record YouTubeBloggerDocument
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
    public YouTubeBloggerFields Fields { get; init; } = new();
    public YouTubeBloggerInterests Interests { get; init; } = new();
    public YouTubeBloggerStatistics Statictics { get; init; } = new();
}

public sealed record YouTubeBloggerFields
{
    public DateTime? PublishedAt { get; init; }
    public int? Posts { get; init; }
    public int? SubscriberCount { get; init; }
    public int? ViewCount { get; init; }
    public YouTubeBloggerInterests Interests { get; init; } = new();
}

public sealed record YouTubeBloggerInterests
{
    public string[] ChannelTags { get; init; } = [];
    public string[] VideoTags { get; init; } = [];
    public string? VideoTitle { get; init; }
}

public sealed record YouTubeBloggerStatistics
{
    public float EngagementRate { get; init; }
    public YouTubeBloggerHalfYearStatistics PerHalfYear { get; init; } = new();
}

public sealed record YouTubeBloggerHalfYearStatistics
{
    public int VideoCount { get; init; }
    public float PostPerDay { get; init; }
    public int AvgLike { get; init; }
    public int AvgView { get; init; }
    public int AvgComment { get; init; }
}
