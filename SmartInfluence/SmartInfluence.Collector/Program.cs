using System.Text;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;

var model = Settings.LoadRequestModel();

await YouTubeApi.CollectUkrainianChannelsAsync(model);
await YouTubeApi.AddPlayListItemsAsync(model);