using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class InfluencerRecommendationService : IInfluencerRecommendationService
{
    private readonly IProductQueryAiService _productQueryAiService;
    private readonly IElasticsearchService _elasticsearchService;

    public InfluencerRecommendationService(
        IProductQueryAiService productQueryAiService,
        IElasticsearchService elasticsearchService)
    {
        _productQueryAiService = productQueryAiService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task<ElasticInfluencerRecommendationResponseModel> RecommendAsync(
        InfluencerRecommendationFiltersModel filters)
    {
        var criteria = await _productQueryAiService.ParseProductDescriptionAsync(filters.Description);
        var channels = await _elasticsearchService.RecommendBloggersAsync(criteria, filters);

        return new ElasticInfluencerRecommendationResponseModel
        {
            Criteria = criteria,
            Channels = channels
        };
    }
}
