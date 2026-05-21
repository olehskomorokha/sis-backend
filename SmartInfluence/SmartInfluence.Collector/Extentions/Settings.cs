using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace SmartInfluence.Collector.Extentions;

public class Settings
{
    public static YouTubeRequestModel _requestModel = new YouTubeRequestModel();
    public static ElasticsearchClient LoadElasticSettings()
    {

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var elasticUrl = configuration["ElasticsearchLocal:Url"]!;
        var elasticIndex = configuration["Elasticsearch:DefaultIndex"]!;

        var client = new ElasticsearchClient(
            new ElasticsearchClientSettings(new Uri(elasticUrl))
                .DefaultIndex(elasticIndex));
        return client;
    }
}