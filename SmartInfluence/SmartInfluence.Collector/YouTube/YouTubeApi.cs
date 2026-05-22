using SmartInfluence.Collector.Extentions;

namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    private static readonly string[] UkrainianChannelQueries = ["авто", "Програмування", "автоогляди", "машини", "електромобілі", "тюнінг", "мотоцикли", "мотоблог", "велосипеди", "дрифт", "перегони", "СТО", "ремонт авто", "детейлінг", "паливо", "новини авто", "автоспорт", "Tesla", "BMW", "Audi", "технології", "гаджети", "смартфони", "ноутбуки", "ПК", "компютери", "ігри", "кіберспорт", "IT", "програмування", "штучний інтелект", "AI", "нейромережі", "робототехніка", "огляди техніки", "android", "iphone", "apple", "windows", "gaming", "valorant", "cs2", "dota2", "minecraft", "fortnite", "pubg", "stream", "twitch", "ютубер", "ігровий блогер", "геймплей", "огляд ігор", "летсплей", "кіберспортсмен", "steam", "xbox", "playstation", "nintendo", "мода", "стиль", "одяг", "fashion", "outfit", "бренди", "шопінг", "аксесуари", "взуття", "макіяж", "краса", "парфуми", "догляд", "косметика", "skincare", "haircare", "манікюр", "барбершоп", "фітнес", "спорт", "тренування", "gym", "бодібілдинг", "біг", "кросфіт", "йога", "пілатес", "харчування", "дієта", "схуднення", "здоровя", "wellness", "марафон", "турнік", "powerlifting", "fitness coach", "їжа", "рецепти", "кулінарія", "кухар", "foodblog", "ресторани", "кафе", "вулична їжа", "десерти", "випічка", "кава", "чай", "бургери", "піца", "суші", "healthy food", "веганство", "гриль", "подорожі", "travel", "туризм", "відпочинок", "готелі", "авіаперельоти", "roadtrip", "кемпінг", "гори", "Карпати", "Європа", "Україна", "туристичний блог", "мандрівки",
        "travel vlog", "пляжі", "backpacking", "музика", "реп", "hiphop", "rock", "pop", "dj", "співак", "співачка", "концерти",
        "музичний блог", "українська музика", "cover", "beatmaker", "producer", "spotify", "music review", "бізнес", "підприємництво", "стартап", "маркетинг", "SMM", "digital marketing", "таргет", "реклама", "бренд", "продажі", "sales", "ecommerce", "dropshipping", "інвестиції", "криптовалюта", "bitcoin", "trading", "фінанси", "освіта", "навчання", "англійська", "математика", "історія", "наука", "physics", "chemistry", "біологія", "edtech", "курси", "саморозвиток", "психологія", "study", "університет", "frontend", "backend", "розваги", "гумор", "меми", "пранки", "реакції", "шоу", "комедія", "tiktok", "shorts", "вірусні відео", "funny", "vlog", "лайфстайл", "storytime", "challenge", "подкаст", "діти", "батьківство", "мама блог", "сімя", "baby", "parenting", "іграшки", "вагітність", "family vlog", "пологи", "mom life", "dad life", "тварини", "pets", "коти", "собаки", "ветеринар", "кінолог", "catlover", "doglover", "animal rescue", "pet care", "ферма", "фотографія", "відеозйомка", "монтаж", "cinematic", "camera", "sony", "canon", "drone", "відеограф", "фотограф", "content creator", "instagram", "reels", "lighting", "filmmaking", "політика", "новини", "журналістика", "війна", "Україна новини", "аналітика", "економіка", "волонтерство", "ЗСУ", "історія України", "telegram", "military", "нерухомість", "будівництво", "ремонт", "дизайн інтерєру", "інтерєр", "архітектура", "DIY", "меблі", "квартира", "будинок", "real estate", "renovation", "home design", "smart home", "Риболовля"];

    public static async Task CollectUkrainianChannelsAsync(YouTubeRequestModel model)
    {
        var collected = new Dictionary<string, UkrainianYouTubeBloggerDto>(StringComparer.OrdinalIgnoreCase);
        //var query = UkrainianChannelQueries[3];
        var query = "Дім";
        var searchRequest = model.Service.Search.List("snippet");
        searchRequest.Q = query;
        searchRequest.Type = "channel";
        searchRequest.RegionCode = "UA";
        searchRequest.MaxResults = Math.Min(50, model.Count);
        
        // пошук блогерів (Search.List)
        var searchResponse = await searchRequest.ExecuteAsync(model.CancellationToken);
        // дістаємо id канала для прдальшого пошуку
        var channelIds = searchResponse.Items?
            .Select(item => item.Snippet?.ChannelId)
            .Where(channelId => !string.IsNullOrWhiteSpace(channelId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var channelsRequest = model.Service.Channels.List("snippet,contentDetails,statistics,topicDetails,brandingSettings");
        channelsRequest.Id = string.Join(",", channelIds ?? []);
        var channelsResponse = await channelsRequest.ExecuteAsync(model.CancellationToken);

        foreach (var channel in channelsResponse.Items ?? [])
        {
            model.CancellationToken.ThrowIfCancellationRequested();

            if (collected.Count >= model.Count)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(channel.Id) || collected.ContainsKey(channel.Id))
            {
                continue;
            }

            var dto = Mapper.MapToBloggerDto(channel);
            collected[channel.Id] = dto;

            await AddBloggerAsync(model, dto);
        }
        Console.WriteLine($"program finished");
    }

    private static async Task AddBloggerAsync(YouTubeRequestModel model, UkrainianYouTubeBloggerDto blogger)
    {
        var response = await model.Elasticsearch.IndexAsync(
            blogger,
            descriptor => descriptor.Index(model.ElasticIndex).Id("youtube" + blogger.ChannelId),
            model.CancellationToken);

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Indexed in Elasticsearch: {blogger.ChannelId} ({blogger.Title})");
            return;
        }

        Console.WriteLine($"Elasticsearch index failed: {blogger.ChannelId}");
        Console.WriteLine(response.ElasticsearchServerError?.Error?.Reason);
    }

    public static async Task AddPlayListItemsAsync(YouTubeRequestModel model)
    {
        var response = await model.Elasticsearch.SearchAsync<UkrainianYouTubeBloggerDto>(s => 
            s.Index(model.ElasticIndex)
            .Size(50)
            .Sort(sort => sort.Field(f => f.IndexedAt, fieldSort => fieldSort.Order(Elastic.Clients.Elasticsearch.SortOrder.Desc))));
        var channelsToProcess = response.Documents
            .Where(x => !string.IsNullOrWhiteSpace(x.ChannelId) && !string.IsNullOrWhiteSpace(x.Uploads))
            .Select(x => new
            {
                x.ChannelId,
                x.Uploads
            })
            .ToList();
        
        foreach (var channel in channelsToProcess)
        {
            model.CancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(channel.Uploads))
            {
                continue;
            }

            Console.WriteLine($"{channel.ChannelId} --> {channel.Uploads}");

            try
            {
                var searchRequest = model.Service.PlaylistItems.List("snippet,contentDetails");
                searchRequest.PlaylistId = channel.Uploads;
                searchRequest.MaxResults = 50;

                var searchResponse = await searchRequest.ExecuteAsync(model.CancellationToken);
                var videoIds = searchResponse.Items?
                    .Select(item => item.Snippet?.ResourceId?.VideoId)
                    .Where(videoId => !string.IsNullOrWhiteSpace(videoId))
                    .ToArray() ?? [];

                Console.WriteLine($"{channel.ChannelId} --> {channel.Uploads} --> {string.Join(", ", videoIds)}");

                foreach (var item in searchResponse.Items ?? [])
                {
                    var dto = Mapper.MapToPlayListItems(item);

                    if (string.IsNullOrWhiteSpace(dto.VideoId))
                    {
                        continue;
                    }

                    var videoId = dto.VideoId;

                    await model.Elasticsearch.IndexAsync(
                        dto,
                        d => d.Index("playlist-items").Id("UploadId:" + channel.Uploads + "-" + videoId),
                        model.CancellationToken);

                    var videoRequest = model.Service.Videos.List("snippet,contentDetails,localizations,recordingDetails,statistics,status,topicDetails");
                    videoRequest.Id = videoId;

                    var videoResponse = await videoRequest.ExecuteAsync(model.CancellationToken);

                    foreach (var video in videoResponse.Items ?? [])
                    {
                        var videoDto = Mapper.MapToVideoDetails(video);

                        await model.Elasticsearch.IndexAsync(
                            videoDto,
                            d => d.Index("youtube-video-details").Id(videoDto.VideoId),
                            model.CancellationToken);
                    }
                }
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Playlist failed: {channel.ChannelId} --> {channel.Uploads}");
                Console.WriteLine(ex.Message);
            }
        }
    }
    
}
