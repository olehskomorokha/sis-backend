using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ClientInfluencerService : IClientInfluencerService
{
    private readonly IClientInfluencerRepository _repository;
    private readonly IInfluencerRepository _influencerRepository;
    public ClientInfluencerService(IClientInfluencerRepository repository, IInfluencerRepository influencerRepository)
    {
        _repository = repository;
        _influencerRepository = influencerRepository;
    }
    
    public async Task<List<ClientInfluencerModel>> GetAllAsync(int clientId)
    {
        /*var clientInfluencers = await _repository.GetAllAsync(clientId);
        
        return clientInfluencers
            .Where(x => x.Influencer != null)
            .Select(x => ClientInfluencerMapper.MapToClientInfluencerCardModel(x, x.Influencer!))
            .ToList();*/
        var clientInfluencers = await _repository.GetAllAsync(clientId);

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
}
