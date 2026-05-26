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
        if (influencer == null)
        {
            influencer = InfluencerMapper.MapToInfluencerEntity(model);
            await _influencerRepository.CreateAsync(influencer);

            var score = InfluencerMapper.MapToInfluencerScoreEntity(model, influencer.Id);
            await _influencerRepository.AddScoreAsync(score);
        }

        var clientInfluencerExists = await _influencerRepository
            .ClientInfluencerExistsAsync(clientId, influencer.Id);
        if (!clientInfluencerExists)
        {
            var clientInfluencer = InfluencerMapper.MapToClientInfluencer(clientId, influencer.Id);
            await _influencerRepository.AddClientInfluencerAsync(clientInfluencer);
        }

        return InfluencerMapper.MapToResponseModel(influencer);
    }

    public async Task<ElasticInfluencerRecommendationResponseModel> RecommendAsync(
        InfluencerRecommendationFiltersModel filters)
    {
        var criteria = await _productQueryAiService.ParseProductDescriptionAsync(filters.Description);
        var channels = await _elasticsearchService.RecommendBloggersAsync(criteria, filters);

        var aiReviewTasks = channels.Select(async channel =>
        {
            if (string.IsNullOrWhiteSpace(channel.ChannelId))
            {
                channel.AiReview = string.Empty;
                return;
            }

            channel.AiReview = await _productQueryAiService.AiChannelReviewAsync(channel.ChannelId);
        });

        await Task.WhenAll(aiReviewTasks);

        return new ElasticInfluencerRecommendationResponseModel
        {
            Channels = channels
        };
    }
}
