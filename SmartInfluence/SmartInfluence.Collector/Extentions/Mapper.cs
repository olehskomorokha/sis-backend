using SmartInfluence.Collector.YouTube;

namespace SmartInfluence.Collector.Extentions;

public class Mapper
{
    public static YouTubeApi.UkrainianYouTubeBloggerDto MapToBloggerDto(Google.Apis.YouTube.v3.Data.Channel channel, List<YouTubeApi.VideoDetailModel> videos)
    {
        var interests = InterestsCalculator.Calculate(channel, videos);
        var fields = FieldsCalculator.Calculate(channel, interests);
        var perHalfYear = StatisticsCalculator.CalculatePerHalfYear(videos);

        return new YouTubeApi.UkrainianYouTubeBloggerDto
        {
            Source = "youtube.com",
            CountryCode = channel.Snippet?.Country,
            ChannelId = channel.Id!,
            ChannelUrl = $"https://www.youtube.com/channel/{channel.Id}",
            Name = channel.Snippet?.Title ?? string.Empty,
            NickName = channel.Snippet?.CustomUrl,
            Description = channel.Snippet?.Description,
            AvatarUrl = channel.Snippet?.Thumbnails?.High?.Url
                           ?? channel.Snippet?.Thumbnails?.Medium?.Url
                           ?? channel.Snippet?.Thumbnails?.Default__?.Url,
            IndexedAt = DateTime.UtcNow,
            Fields = fields,
            Interests = interests,
            Statictics = new YouTubeApi.Statictics()
            {
                PerHalfYear = perHalfYear
            }
            
        };
    }

    public static YouTubeApi.PlayListItemsModel MapToPlayListItems(Google.Apis.YouTube.v3.Data.PlaylistItem model)
    {
        return new YouTubeApi.PlayListItemsModel()
        {
            VideoId = model.Snippet.ResourceId.VideoId,
            PublishedAt = model.Snippet.PublishedAtRaw,
            ChannelId = model.Snippet.ChannelId,
            PlaylistId = model.Snippet.PlaylistId,
            Title = model.Snippet.Title,
            Description = model.Snippet.Description,
            ChannelTitle = model.Snippet.ChannelTitle
        };
    }

    public static YouTubeApi.VideoDetailModel MapToVideoDetails(Google.Apis.YouTube.v3.Data.Video video)
    {
        return new YouTubeApi.VideoDetailModel
        {
            VideoId = video.Id ?? string.Empty,
            ChannelId = video.Snippet?.ChannelId,
            ChannelTitle = video.Snippet?.ChannelTitle,
            Title = video.Snippet?.Title,
            Description = video.Snippet?.Description,
            PublishedAt = GetPublishedAtOrEmpty(video.Snippet),
            ThumbnailUrl = GetThumbnailUrl(video.Snippet?.Thumbnails),
            Tags = string.Join(",", video.Snippet?.Tags ?? []),
            TopicCategories = string.Join(",", ExtractTopicCategories(video.TopicDetails?.TopicCategories)),
            CategoryId = video.Snippet?.CategoryId,
            DefaultLanguage = video.Snippet?.DefaultLanguage,
            Duration = video.ContentDetails?.Duration,
            Definition = video.ContentDetails?.Definition,
            Caption = video.ContentDetails?.Caption,
            LicensedContent = video.ContentDetails?.LicensedContent ?? false,
            UploadStatus = video.Status?.UploadStatus,
            ViewCount = video.Statistics?.ViewCount,
            LikeCount = video.Statistics?.LikeCount,
            FavoriteCount = video.Statistics?.FavoriteCount,
            CommentCount = video.Statistics?.CommentCount
        };
    }
    private static string GetPublishedAtOrEmpty(Google.Apis.YouTube.v3.Data.VideoSnippet? snippet)
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

    private static string? GetThumbnailUrl(Google.Apis.YouTube.v3.Data.ThumbnailDetails? thumbnails)
    {
        return thumbnails?.Maxres?.Url
               ?? thumbnails?.Standard?.Url
               ?? thumbnails?.High?.Url
               ?? thumbnails?.Medium?.Url
               ?? thumbnails?.Default__?.Url;
    }

    private static IEnumerable<string> ExtractTopicCategories(IList<string>? topicCategoryUrls)
    {
        if (topicCategoryUrls is null)
        {
            return [];
        }

        return topicCategoryUrls
            .Select(ExtractLastPathSegment)
            .Where(topic => !string.IsNullOrWhiteSpace(topic))
            .Select(topic => topic!);
    }

    private static string? ExtractLastPathSegment(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null;
        }

        var segment = uri.Segments.LastOrDefault()?.Trim('/');
        return string.IsNullOrWhiteSpace(segment) ? null : Uri.UnescapeDataString(segment);
    }
}
