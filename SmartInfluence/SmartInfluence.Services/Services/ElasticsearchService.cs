using Elastic.Clients.Elasticsearch;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    
    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<List<string>> GetAllBloggerTags()
    {
        var response = await _client.SearchAsync<object>(s => s
            .Index("blogger")
            .Size(0)
            .Aggregations(a => a
                .Terms("unique_topic_categories", t => t
                    .Field("topicCategories.keyword")
                    .Size(1000)
                )
            )
        );

        var aggregation = response.Aggregations
            .GetStringTerms("unique_topic_categories");

        var tags = aggregation.Buckets
            .Select(b => b.Key.ToString())
            .ToList();
        
        var uniqueTags = tags
            .SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .ToList();
        return uniqueTags;
    }

    public async Task<List<InfluencerResponseModel>> GetByFilters(InfluencerRecommendationRequestModel model)
    {
        
    }
}