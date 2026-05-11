using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class AudienceService : IAudienceService
{
    private readonly IAudienceRepository _audienceRepository;

    public AudienceService(IAudienceRepository audienceRepository)
    {
        _audienceRepository = audienceRepository;
    }

    public async Task<AudienceResponseModel?> GetByInfluencerIdAsync(int influencerId)
    {
        var audience = await _audienceRepository.GetByInfluencerIdAsync(influencerId);
        return audience == null ? null : AudienceMapper.MapToResponseModel(audience);
    }
}
