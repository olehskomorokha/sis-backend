namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    private static readonly string[] UkrainianChannelQueries =
    [
        "Риболовля"
    ];

    public static async Task<IReadOnlyList<UkrainianYouTubeBloggerDto>> ExportUkrainianBloggersToJsonAsync(
        string apiKey,
        int count,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero.");
        }

        var service = new Google.Apis.YouTube.v3.YouTubeService(new Google.Apis.Services.BaseClientService.Initializer
        {
            ApiKey = apiKey,
            ApplicationName = "SmartInfluence.Collector"
        });

        var channels = await CollectUkrainianChannelsAsync(service, count, cancellationToken);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = System.Text.Json.JsonSerializer.Serialize(
            channels,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(outputPath, json, cancellationToken);

        return channels;
    }

    private static async Task<IReadOnlyList<UkrainianYouTubeBloggerDto>> CollectUkrainianChannelsAsync(Google.Apis.YouTube.v3.YouTubeService service, int count, CancellationToken cancellationToken)
    {
        var collected = new Dictionary<string, UkrainianYouTubeBloggerDto>(StringComparer.OrdinalIgnoreCase);

        foreach (var query in UkrainianChannelQueries)
        {
            if (collected.Count >= count)
            {
                break;
            }

            var searchRequest = service.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "channel";
            searchRequest.RegionCode = "UA";
            searchRequest.MaxResults = 25;

            var searchResponse = await searchRequest.ExecuteAsync(cancellationToken);
            var channelIds = searchResponse.Items?
                .Select(item => item.Snippet?.ChannelId)
                .Where(channelId => !string.IsNullOrWhiteSpace(channelId))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (channelIds is null || channelIds.Length == 0)
            {
                continue;
            }

            var channelsRequest = service.Channels.List("snippet,statistics");
            channelsRequest.Id = string.Join(",", channelIds);
            var channelsResponse = await channelsRequest.ExecuteAsync(cancellationToken);

            foreach (var channel in channelsResponse.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();

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

                if (collected.Count >= count)
                {
                    break;
                }
            }
        }

        return collected.Values
            .OrderByDescending(channel => channel.SubscriberCount ?? 0)
            .ThenBy(channel => channel.Title, StringComparer.OrdinalIgnoreCase)
            .Take(count)
            .ToArray();
    }
}
