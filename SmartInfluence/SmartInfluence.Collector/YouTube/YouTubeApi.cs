using Google.Apis.YouTube.v3.Data;
using SmartInfluence.Collector.Extentions;

namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    public static async Task<List<Channel>> CollectUkrainianChannelsAsync(YouTubeRequestModel model, string query)
    {
        var searchRequest = model.Service.Search.List("snippet");
        searchRequest.Q = query;
        searchRequest.RelevanceLanguage = "uk";
        searchRequest.Type = "video";
        searchRequest.RegionCode = "UA";
        searchRequest.MaxResults = 50;
        
        // пошук блогерів (Search.List)
        var searchResponse = await searchRequest.ExecuteAsync(model.CancellationToken);
        // дістаємо id каналу для подальшого пошуку
        var channelIds = searchResponse.Items?
            .Select(item => item.Snippet?.ChannelId)
            .Where(channelId => !string.IsNullOrWhiteSpace(channelId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (channelIds is null || channelIds.Length == 0)
        {
            return [];
        }

        var channelsRequest = model.Service.Channels.List("snippet,contentDetails,statistics,topicDetails,brandingSettings");
        channelsRequest.Id = string.Join(",", channelIds);
        var channelsResponse = await channelsRequest.ExecuteAsync(model.CancellationToken);
        
        return channelsResponse.Items?.ToList() ?? [];
    }

    public static async Task<List<string>> GetChannelPlayListItemsAsync(YouTubeRequestModel model, string channelId)
    {
        var result = new List<string>();
        var cutoff = DateTimeOffset.UtcNow.AddMonths(-6);
        string? pageToken = null;
        do
        {
            var searchRequest = model.Service.PlaylistItems.List("snippet,contentDetails");
            searchRequest.PlaylistId = StringConverter.ConvertChannelIdToPlaylistId(channelId);
            searchRequest.MaxResults = 50;
            searchRequest.PageToken = pageToken;
            
            var searchResponse = await searchRequest.ExecuteAsync(model.CancellationToken);

            foreach (var item in searchResponse.Items ?? [])
            {
                var publishedAt = item.ContentDetails?.VideoPublishedAtDateTimeOffset;

                if (publishedAt is null)
                {
                    continue;
                }

                if (publishedAt < cutoff)
                {
                    return result;
                }
                var videoId = item.Snippet?.ResourceId?.VideoId;
                if (!string.IsNullOrWhiteSpace(videoId))
                {
                    result.Add(videoId);
                }
            }

            pageToken = searchResponse.NextPageToken;
            
        } while (!string.IsNullOrWhiteSpace(pageToken));
       
        return result;
    }

    public static async Task<List<YouTubeApi.VideoDetailModel>> GetChannelVideoDetailsAsync(YouTubeRequestModel model, List<string> videoIds)
    {
        var result = new List<YouTubeApi.VideoDetailModel>();

        foreach (var batch in videoIds.Chunk(50))
        {
            var videoRequest = model.Service.Videos.List(
                "snippet,contentDetails,localizations,recordingDetails,statistics,status,topicDetails");

            videoRequest.Id = string.Join(",", batch);

            var videoResponse = await videoRequest.ExecuteAsync(model.CancellationToken);

            foreach (var video in videoResponse.Items ?? [])
            {
                result.Add(Mapper.MapToVideoDetails(video));
            }
        }
        return result;
    }
}
