using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class InfluencerService : IInfluencerService
{
    private readonly IInfluencerRepository _influencerRepository;

    public InfluencerService(IInfluencerRepository influencerRepository)
    {
        _influencerRepository = influencerRepository;
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
}
