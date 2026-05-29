using SmartInfluence.Data.Entities;
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


    public async Task<List<InfluencerResponseModel>> GetByClientIdAsync(int clientId)
    {
        var influencers = await _influencerRepository.GetByClientIdAsync(clientId);
        var scores = await _influencerRepository.GetLatestScoresByInfluencerIdsAsync(
            influencers.Select(x => x.Id));
        var scoresByInfluencerId = scores.ToDictionary(x => x.InfluencerId);

        return influencers
            .Select(influencer => InfluencerMapper.MapToResponseModel(
                influencer,
                scoresByInfluencerId.GetValueOrDefault(influencer.Id)))
            .ToList();
    }
    public async Task<List<InfluencerResponseModel>> GetAllAsync()
    {
        var influencers = await _influencerRepository.GetAllAsync();
        return influencers.Select(influencer => InfluencerMapper.MapToResponseModel(influencer)).ToList();
    }

    public async Task<InfluencerResponseModel?> GetByIdAsync(int id)
    {
        var influencer = await _influencerRepository.GetByIdAsync(id);
        return influencer == null ? null : InfluencerMapper.MapToResponseModel(influencer);
    }

    public async Task<InfluencerResponseModel> SaveRecommendedAsync(RecommendedChannelModel model, int clientId)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(model.ChannelId))
        {
            throw new ArgumentException("ChannelId is required.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(model.ChannelName))
        {
            throw new ArgumentException("ChannelName is required.", nameof(model));
        }

        var influencer = await _influencerRepository.GetByInfluencerIdAsync(model.ChannelId);
        InfluencerScore? score = null;

        if (influencer == null)
        {
            influencer = InfluencerMapper.MapToInfluencerEntity(model);
            await _influencerRepository.CreateAsync(influencer);

            score = InfluencerMapper.MapToInfluencerScoreEntity(model, influencer.Id);
            await _influencerRepository.AddScoreAsync(score);
        }
        else
        {
            score = (await _influencerRepository.GetLatestScoresByInfluencerIdsAsync([influencer.Id]))
                .FirstOrDefault();
        }

        var clientInfluencerExists = await _influencerRepository
            .ClientInfluencerExistsAsync(clientId, influencer.Id);
        if (!clientInfluencerExists)
        {
            var clientInfluencer = ClientInfluencerMapper.MapToClientInfluencer(clientId, model, influencer.Id);
            await _influencerRepository.AddClientInfluencerAsync(clientInfluencer);
        }

        return InfluencerMapper.MapToResponseModel(influencer, score);
    }

    public async Task<List<RecommendedChannelModel>> RecommendAsync(
        InfluencerRecommendationFiltersModel filters)
    {
        var criteria = await _productQueryAiService.ParseProductDescriptionAsync(filters.Description);
        var channels = await _elasticsearchService.RecommendBloggersAsync(criteria, filters);
        
        return channels;
    }
}
