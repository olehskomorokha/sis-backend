using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;

namespace SmartInfluence.Collector.Extentions;

public static class Settings
{
    public static YouTubeRequestModel RequestModel { get; private set; } = null!;

    public static YouTubeRequestModel LoadRequestModel()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var youtubeApiKey = configuration["YouTube:ApiKey"];

        var elasticUrl = configuration["Elasticsearch:Url"]!;
        var elasticIndex = configuration["Elasticsearch:DefaultIndex"] ?? "youtube";
        var elasticApiKey = configuration["Elasticsearch:ApiKey"];

        var elasticSettings = new ElasticsearchClientSettings(new Uri(elasticUrl))
            .DefaultIndex(elasticIndex)
            .Authentication(new ApiKey(elasticApiKey));

        var elasticClient = new ElasticsearchClient(elasticSettings);

        var youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = youtubeApiKey,
            ApplicationName = "SmartInfluence.Collector"
        });

        RequestModel = new YouTubeRequestModel
        {
            Service = youtubeService,
            Elasticsearch = elasticClient,
            ElasticIndex = elasticIndex
        };

        return RequestModel;
    }
}
