using SmartInfluence.Collector.Extentions;

namespace SmartInfluence.Collector.YouTube;

public class YouTubeElasticService
{
    public static async Task AddBloggerAsync(YouTubeRequestModel model, YouTubeApi.UkrainianYouTubeBloggerDto blogger)
    {
        var response = await model.Elasticsearch.IndexAsync(
            blogger,
            descriptor => descriptor.Index("youtube").Id("youtube-" + blogger.ChannelId),
            model.CancellationToken);

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Indexed in Elasticsearch: {blogger.ChannelId} ({blogger.Name})");
            return;
        }

        Console.WriteLine($"Elasticsearch index failed: {blogger.ChannelId}");
        Console.WriteLine(response.ElasticsearchServerError?.Error?.Reason);
    }
}