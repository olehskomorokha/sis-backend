using System.Text;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;

var model = Settings.LoadRequestModel();

var channels = await YouTubeApi.CollectUkrainianChannelsAsync(model);

foreach (var channel in channels)
{
    var videoIds = YouTubeApi.GetChannelPlayListItemsAsync(model, channel.Id);
    var videoDetails = await YouTubeApi.GetChannelVideoDetailsAsync(model, videoIds.Result);
    var mappedBlogger = Mapper.MapToBloggerDto(channel, videoDetails);
    await YouTubeElasticService.AddBloggerAsync(model, mappedBlogger);
}

