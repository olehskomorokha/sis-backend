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

        public ulong? SubscriberCount { get; init; }

        public ulong? VideoCount { get; init; }

        public ulong? ViewCount { get; init; }

        public string? ThumbnailUrl { get; init; }

        public required string SourceQuery { get; init; }

        public required string ChannelUrl { get; init; }
    }
}
