using Elastic.Clients.Elasticsearch;
using SmartInfluence.Services.Interfaces;

namespace SmartInfluence.Services.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    
    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }
}