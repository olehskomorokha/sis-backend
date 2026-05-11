using SmartInfluence.Data.Entities;

namespace SmartInfluence.Data.Interfaces;

public interface IAudienceRepository
{
    public Task<Audience?> GetByInfluencerIdAsync(int influencerId);
}
