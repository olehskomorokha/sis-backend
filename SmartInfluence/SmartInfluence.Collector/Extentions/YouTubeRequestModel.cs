using Elastic.Clients.Elasticsearch;

namespace SmartInfluence.Collector.Extentions;

public class YouTubeRequestModel
{
    public required Google.Apis.YouTube.v3.YouTubeService Service { get; init; }

    public required ElasticsearchClient Elasticsearch { get; init; }

    public required string ElasticIndex { get; init; }

    public CancellationToken CancellationToken { get; init; }
}