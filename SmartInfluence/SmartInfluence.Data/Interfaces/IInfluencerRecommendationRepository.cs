using SmartInfluence.Data.Models;

namespace SmartInfluence.Data.Interfaces;

public interface IInfluencerRecommendationRepository
{
    Task<List<InfluencerRecommendationCandidateData>> GetCandidatesAsync();
}
