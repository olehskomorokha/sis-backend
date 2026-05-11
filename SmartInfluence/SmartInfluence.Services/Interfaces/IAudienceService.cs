using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IAudienceService
{
    public Task<AudienceResponseModel?> GetByInfluencerIdAsync(int influencerId);
}
