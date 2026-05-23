using System.Text;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;

var collected = new Dictionary<string, YouTubeApi.UkrainianYouTubeBloggerDto>(StringComparer.OrdinalIgnoreCase);
var model = Settings.LoadRequestModel();

var channels = await YouTubeApi.CollectUkrainianChannelsAsync(model);
foreach (var channel in channels)
{
    var dto = Mapper.MapToBloggerDto(channel);
    collected[channel.Id] = dto;

    await AddBloggerAsync(model, dto);
}

await YouTubeApi.AddPlayListItemsAsync(model);

static async Task AddBloggerAsync(YouTubeRequestModel model, YouTubeApi.UkrainianYouTubeBloggerDto blogger)
{
    var response = await model.Elasticsearch.IndexAsync(
        blogger,
        descriptor => descriptor.Index(model.ElasticIndex).Id("youtube" + blogger.ChannelId),
        model.CancellationToken);

    if (response.IsValidResponse)
    {
        Console.WriteLine($"Indexed in Elasticsearch: {blogger.ChannelId} ({blogger.Name})");
        return;
    }

    Console.WriteLine($"Elasticsearch index failed: {blogger.ChannelId}");
    Console.WriteLine(response.ElasticsearchServerError?.Error?.Reason);
}