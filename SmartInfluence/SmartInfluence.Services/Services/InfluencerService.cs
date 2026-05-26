using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class InfluencerService : IInfluencerService
{
    private readonly IInfluencerRepository _influencerRepository;
    private readonly IProductQueryAiService _productQueryAiService;
    private readonly IElasticsearchService _elasticsearchService;

    public InfluencerService(IInfluencerRepository influencerRepository,
        IProductQueryAiService productQueryAiService,
        IElasticsearchService elasticsearchService)
    {
        _influencerRepository = influencerRepository;
        _productQueryAiService = productQueryAiService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task<List<InfluencerResponseModel>> GetAllAsync()
    {
        var influencers = await _influencerRepository.GetAllAsync();
        return influencers.Select(InfluencerMapper.MapToResponseModel).ToList();
    }

    public async Task<InfluencerResponseModel?> GetByIdAsync(int id)
    {
        var influencer = await _influencerRepository.GetByIdAsync(id);
        return influencer == null ? null : InfluencerMapper.MapToResponseModel(influencer);
    }

    public async Task AddInfluencer()
    {
        
    }
    public async Task<ElasticInfluencerRecommendationResponseModel> RecommendAsync(
        InfluencerRecommendationFiltersModel filters)
    {
        var criteria = await _productQueryAiService.ParseProductDescriptionAsync(filters.Description);
        var channels = await _elasticsearchService.RecommendBloggersAsync(criteria, filters);

        return new ElasticInfluencerRecommendationResponseModel
        {
            Channels = channels
        };
    }
}
