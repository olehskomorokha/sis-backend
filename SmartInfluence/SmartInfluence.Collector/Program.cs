using System.Text;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;
const int UnitsLimit = 300;
string[] UkrainianChannelQueries =
[
    "ігри", "освіта", "бізнес", "краса", "технології", "ігри", "музика",
    "фітнес", "освіта", "бізнес", "мода", "краса", "риболовля", "садівництво",
    "психологія", "гумор", "кіно", "тварини", "будівництво", "наука"
];
var usedUnits = 0;

var model = Settings.LoadRequestModel();

foreach (var query in UkrainianChannelQueries)
{
    if (usedUnits > UnitsLimit - 101)
    {
        Console.WriteLine($"Total used  units: {usedUnits}");
        return;
    }

    var channels = await YouTubeApi.CollectUkrainianChannelsAsync(model, query);
        
    // search.list + channels.list
    usedUnits += 101;
    
    foreach (var channel in channels)
    {
        // playlistItems.list
        usedUnits += 1;
            
        var videoIds = await YouTubeApi.GetChannelPlayListItemsAsync(model, channel.Id);
            
        // videos.list
        usedUnits += (int)Math.Ceiling(videoIds.Count / 50.0);
            
        var videoDetails = await YouTubeApi.GetChannelVideoDetailsAsync(model, videoIds);
        var mappedBlogger = Mapper.MapToBloggerDto(channel, videoDetails);
            
        Console.WriteLine($"usedUnits: {usedUnits}");
            
        await YouTubeElasticService.AddBloggerAsync(model, mappedBlogger);
    }
}
