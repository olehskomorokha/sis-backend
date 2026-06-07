using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ClientInfluencerService : IClientInfluencerService
{
    private readonly IClientInfluencerRepository _repository;
    private readonly IInfluencerRepository _influencerRepository;

    public ClientInfluencerService(
        IClientInfluencerRepository repository,
        IInfluencerRepository influencerRepository)
    {
        _repository = repository;
        _influencerRepository = influencerRepository;
    }

    public async Task<List<ClientInfluencerModel>> GetAllAsync(int clientId)
    {
        var clientInfluencers = await _repository.GetAllClientInfluencersAsync(clientId);

        var influencerIds = clientInfluencers
            .Where(x => x.Influencer != null)
            .Select(x => x.Influencer!.Id)
            .ToList();

        var scores = await _influencerRepository.GetLatestScoresByInfluencerIdsAsync(influencerIds);
        var scoresByInfluencerId = scores.ToDictionary(x => x.InfluencerId);

        return clientInfluencers
            .Where(x => x.Influencer != null)
            .Select(x => ClientInfluencerMapper.MapToClientInfluencerCardModel(
                x,
                x.Influencer!,
                scoresByInfluencerId.GetValueOrDefault(x.Influencer!.Id)))
            .ToList();
    }

    public async Task<bool> UpdateAsync(int clientId, int influencerId, UpdateClientInfluencerModel model)
    {
        var clientInfluencer = await _repository.GetByClientAndInfluencerAsync(clientId, influencerId);
        if (clientInfluencer == null)
        {
            return false;
        }

        if (model.TotalScore.HasValue)
            clientInfluencer.TotalScore = model.TotalScore.Value;

        if (model.BrandFitScore.HasValue)
            clientInfluencer.BrandFitScore = model.BrandFitScore.Value;

        if (model.AiReview != null)
            clientInfluencer.AiReview = model.AiReview;

        if (model.Status.HasValue)
            clientInfluencer.Status = model.Status.Value;

        if (model.PredictedEngagement.HasValue)
            clientInfluencer.PredictedEngagement = model.PredictedEngagement.Value;

        await _repository.UpdateAsync(clientInfluencer);
        return true;
    }

    public async Task<bool> DeleteAsync(int clientId, int influencerId)
    {
        var clientInfluencer = await _repository.GetByClientAndInfluencerAsync(clientId, influencerId);
        if (clientInfluencer == null)
        {
            return false;
        }

        await _repository.DeleteAsync(clientInfluencer);

        var stillUsedByAnyClient = await _repository.ExistsByInfluencerIdAsync(influencerId);
        if (stillUsedByAnyClient)
        {
            return true;
        }

        var influencer = await _influencerRepository.GetByIdAsync(influencerId);
        if (influencer == null)
        {
            return true;
        }

        await _influencerRepository.DeleteScoresByInfluencerIdAsync(influencerId);
        await _influencerRepository.DeleteAsync(influencer);

        return true;
    }
}
