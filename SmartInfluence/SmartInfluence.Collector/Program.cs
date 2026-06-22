using System.Text;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;
const int unitsLimit = 300;
string[] queries =
[
    "Університет", "інвестиції", "новини", "політика", "історія", "література", "дизайн", "фотографія", "мобільні ігри", "програмування", "штучний інтелект", "подорожі", "автомобілі", "електроніка", "гаджети",

    "наука", "астрономія", "космос", "освіта", "англійська мова", "математика", "бізнес", "стартапи", "маркетинг", "психологія", "мотивація", "саморозвиток", "кулінарія", "рецепти", "рибалка", "полювання", "туризм", "велоспорт", "волейбол", "баскетбол", "теніс",

    "єдиноборства", "біг", "фітнес", "gym", "музика", "кіно", "серіали", "аніме", "криптовалюта", "blockchain", "Docker", "ASP.NET", "C#", "PHP", "JavaScript", "React", "Angular", "Azure", "DevOps", "кібербезпека", "машинне навчання", "NLP", "робототехніка"
];
var usedUnits = 0;

var model = Settings.LoadRequestModel();

foreach (var query in queries)
{
    if (usedUnits > unitsLimit - 101)
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
