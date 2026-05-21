using SmartInfluence.Collector.Extentions;

namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    private static readonly string[] UkrainianChannelQueries =
    [
        "Риболовля"
    ];
    
    public static async Task<IReadOnlyList<UkrainianYouTubeBloggerDto>> CollectUkrainianChannelsAsync(YouTubeRequestModel model)
    {
        var collected = new Dictionary<string, UkrainianYouTubeBloggerDto>(StringComparer.OrdinalIgnoreCase);

        foreach (var query in UkrainianChannelQueries)
        {
            if (collected.Count >= model.Count)
            {
                break;
            }

            var searchRequest = model.Service.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "channel";
            searchRequest.RegionCode = "UA";
            searchRequest.MaxResults = 25;

            var searchResponse = await searchRequest.ExecuteAsync(model.CancellationToken);
            var channelIds = searchResponse.Items?
                .Select(item => item.Snippet?.ChannelId)
                .Where(channelId => !string.IsNullOrWhiteSpace(channelId))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (channelIds is null || channelIds.Length == 0)
            {
                continue;
            }

            var channelsRequest = model.Service.Channels.List("snippet,statistics");
            channelsRequest.Id = string.Join(",", channelIds);
            var channelsResponse = await channelsRequest.ExecuteAsync(model.CancellationToken);

            foreach (var channel in channelsResponse.Items)
            {
                model.CancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(channel.Id) || collected.ContainsKey(channel.Id))
                {
                    continue;
                }

                var dto = new UkrainianYouTubeBloggerDto
                {
                    ChannelId = channel.Id,
                    Title = channel.Snippet?.Title ?? string.Empty,
                    Description = channel.Snippet?.Description,
                    CustomUrl = channel.Snippet?.CustomUrl,
                    PublishedAt = channel.Snippet?.PublishedAtDateTimeOffset?.ToString("O"),
                    Country = channel.Snippet?.Country,
                    SubscriberCount = channel.Statistics?.SubscriberCount,
                    VideoCount = channel.Statistics?.VideoCount,
                    ViewCount = channel.Statistics?.ViewCount,
                    ThumbnailUrl = channel.Snippet?.Thumbnails?.High?.Url
                        ?? channel.Snippet?.Thumbnails?.Medium?.Url
                        ?? channel.Snippet?.Thumbnails?.Default__?.Url,
                    SourceQuery = query,
                    ChannelUrl = $"https://www.youtube.com/channel/{channel.Id}"
                };

                collected[channel.Id] = dto;

                if (collected.Count >= model.Count)
                {
                    break;
                }
            }
        }

        return collected.Values
            .OrderByDescending(channel => channel.SubscriberCount ?? 0)
            .ThenBy(channel => channel.Title, StringComparer.OrdinalIgnoreCase)
            .Take(model.Count)
            .ToArray();
    }
}
