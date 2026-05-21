using SmartInfluence.Collector.YouTube;

namespace SmartInfluence.Collector.Extentions;

public class Mapper
{
    public static YouTubeApi.UkrainianYouTubeBloggerDto MapToBloggerDto(Google.Apis.YouTube.v3.Data.Channel channel)
    {
        return new YouTubeApi.UkrainianYouTubeBloggerDto
        {
            ChannelId = channel.Id!,
            Title = channel.Snippet?.Title ?? string.Empty,
            Description = channel.Snippet?.Description,
            CustomUrl = channel.Snippet?.CustomUrl,
            PublishedAt = GetPublishedAtOrEmpty(channel.Snippet),
            Country = channel.Snippet?.Country,
            Uploads = channel.ContentDetails?.RelatedPlaylists?.Uploads ?? string.Empty,
            VideoCount = channel.Statistics?.VideoCount,
            SubscriberCount = channel.Statistics?.SubscriberCount,
            ViewCount = channel.Statistics?.ViewCount,
            Statictics = new YouTubeApi.Statictics
            {
                ViewCount = channel.Statistics?.ViewCount is > int.MaxValue
                    ? int.MaxValue
                    : (int)(channel.Statistics?.ViewCount ?? 0),
                SubscriberCount = channel.Statistics?.SubscriberCount,
                HiddenSubscriberCount = channel.Statistics?.HiddenSubscriberCount ?? false,
                VideoCount = channel.Statistics?.VideoCount
            },
            IndexedAt = DateTime.UtcNow,
            ThumbnailUrl = channel.Snippet?.Thumbnails?.High?.Url
                           ?? channel.Snippet?.Thumbnails?.Medium?.Url
                           ?? channel.Snippet?.Thumbnails?.Default__?.Url,
            ChannelUrl = $"https://www.youtube.com/channel/{channel.Id}"
        };
    }

    public static YouTubeApi.PlayListItems MapToPlayListItems(Google.Apis.YouTube.v3.Data.PlaylistItem model)
    {
        return new YouTubeApi.PlayListItems()
        {
            VideoId = model.Snippet.ResourceId.VideoId,
            PublishedAt = model.Snippet.PublishedAtRaw,
            ChannelId = model.Snippet.ChannelId,
            Title = model.Snippet.Title,
            Description = model.Snippet.Description,
            ChannelTitle = model.Snippet.ChannelTitle
        };
    }
    private static string GetPublishedAtOrEmpty(Google.Apis.YouTube.v3.Data.ChannelSnippet? snippet)
    {
        if (snippet is null)
        {
            return string.Empty;
        }

        try
        {
            return snippet.PublishedAtDateTimeOffset?.ToString("O") ?? string.Empty;
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }
}