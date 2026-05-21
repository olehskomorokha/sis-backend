namespace SmartInfluence.Collector.YouTube;

public static partial class YouTubeApi
{
    private static readonly string[] UkrainianChannelQueries = ["авто", "автомобілі", "автоогляди", "машини", "електромобілі", "тюнінг", "мотоцикли", "мотоблог", "велосипеди", "дрифт", "перегони", "СТО", "ремонт авто", "детейлінг", "паливо", "новини авто", "автоспорт", "Tesla", "BMW", "Audi", "технології", "гаджети", "смартфони", "ноутбуки", "ПК", "компютери", "ігри", "кіберспорт", "IT", "програмування", "штучний інтелект", "AI", "нейромережі", "робототехніка", "огляди техніки", "android", "iphone", "apple", "windows", "gaming", "valorant", "cs2", "dota2", "minecraft", "fortnite", "pubg", "stream", "twitch", "ютубер", "ігровий блогер", "геймплей", "огляд ігор", "летсплей", "кіберспортсмен", "steam", "xbox", "playstation", "nintendo", "мода", "стиль", "одяг", "fashion", "outfit", "бренди", "шопінг", "аксесуари", "взуття", "макіяж", "краса", "парфуми", "догляд", "косметика", "skincare", "haircare", "манікюр", "барбершоп", "фітнес", "спорт", "тренування", "gym", "бодібілдинг", "біг", "кросфіт", "йога", "пілатес", "харчування", "дієта", "схуднення", "здоровя", "wellness", "марафон", "турнік", "powerlifting", "fitness coach", "їжа", "рецепти", "кулінарія", "кухар", "foodblog", "ресторани", "кафе", "вулична їжа", "десерти", "випічка", "кава", "чай", "бургери", "піца", "суші", "healthy food", "веганство", "гриль", "подорожі", "travel", "туризм", "відпочинок", "готелі", "авіаперельоти", "roadtrip", "кемпінг", "гори", "Карпати", "Європа", "Україна", "туристичний блог", "мандрівки",
        "travel vlog", "пляжі", "backpacking", "музика", "реп", "hiphop", "rock", "pop", "dj", "співак", "співачка", "концерти",
        "музичний блог", "українська музика", "cover", "beatmaker", "producer", "spotify", "music review", "бізнес", "підприємництво", "стартап", "маркетинг", "SMM", "digital marketing", "таргет", "реклама", "бренд", "продажі", "sales", "ecommerce", "dropshipping", "інвестиції", "криптовалюта", "bitcoin", "trading", "фінанси", "освіта", "навчання", "англійська", "математика", "історія", "наука", "physics", "chemistry", "біологія", "edtech", "курси", "саморозвиток", "психологія", "study", "університет", "frontend", "backend", "розваги", "гумор", "меми", "пранки", "реакції", "шоу", "комедія", "tiktok", "shorts", "вірусні відео", "funny", "vlog", "лайфстайл", "storytime", "challenge", "подкаст", "діти", "батьківство", "мама блог", "сімя", "baby", "parenting", "іграшки", "вагітність", "family vlog", "пологи", "mom life", "dad life", "тварини", "pets", "коти", "собаки", "ветеринар", "кінолог", "catlover", "doglover", "animal rescue", "pet care", "ферма", "фотографія", "відеозйомка", "монтаж", "cinematic", "camera", "sony", "canon", "drone", "відеограф", "фотограф", "content creator", "instagram", "reels", "lighting", "filmmaking", "політика", "новини", "журналістика", "війна", "Україна новини", "аналітика", "економіка", "волонтерство", "ЗСУ", "історія України", "telegram", "military", "нерухомість", "будівництво", "ремонт", "дизайн інтерєру", "інтерєр", "архітектура", "DIY", "меблі", "квартира", "будинок", "real estate", "renovation", "home design", "smart home", "Риболовля"];

    public static async Task<IReadOnlyList<UkrainianYouTubeBloggerDto>> ExportUkrainianBloggersToJsonAsync(
        string apiKey,
        int count,
        string outputPath,
        Func<UkrainianYouTubeBloggerDto, CancellationToken, Task>? onChannelCollected = null,
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

        var channels = await CollectUkrainianChannelsAsync(service, count, onChannelCollected, cancellationToken);

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

    private static async Task<IReadOnlyList<UkrainianYouTubeBloggerDto>> CollectUkrainianChannelsAsync(
        Google.Apis.YouTube.v3.YouTubeService service,
        int count,
        Func<UkrainianYouTubeBloggerDto, CancellationToken, Task>? onChannelCollected,
        CancellationToken cancellationToken)
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

            var channelsRequest = service.Channels.List("snippet,contentDetails,statistics,topicDetails, brandingSettings");
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
                    Uploads = channel.ContentDetails?.RelatedPlaylists?.Uploads ?? string.Empty,
                    VideoCount = channel.Statistics?.VideoCount,
                    SubscriberCount = channel.Statistics?.SubscriberCount,
                    ViewCount = channel.Statistics?.ViewCount,
                    Statictics = new Statictics
                    {
                        ViewCount = channel.Statistics?.ViewCount is > int.MaxValue
                            ? int.MaxValue
                            : (int)(channel.Statistics?.ViewCount ?? 0),
                        SubscriberCount = channel.Statistics?.SubscriberCount,
                        HiddenSubscriberCount = channel.Statistics?.HiddenSubscriberCount ?? false,
                        VideoCount = channel.Statistics?.VideoCount
                    },
                    ThumbnailUrl = channel.Snippet?.Thumbnails?.High?.Url
                        ?? channel.Snippet?.Thumbnails?.Medium?.Url
                        ?? channel.Snippet?.Thumbnails?.Default__?.Url,
                    SourceQuery = query,
                    ChannelUrl = $"https://www.youtube.com/channel/{channel.Id}"
                };

                collected[channel.Id] = dto;
                if (onChannelCollected is not null)
                {
                    await onChannelCollected(dto, cancellationToken);
                }

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
