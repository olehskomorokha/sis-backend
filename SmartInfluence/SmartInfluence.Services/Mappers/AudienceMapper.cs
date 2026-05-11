using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class AudienceMapper
{
    public static AudienceResponseModel MapToResponseModel(Audience audience)
    {
        return new AudienceResponseModel
        {
            Id = audience.Id,
            InfluencerId = audience.InfluencerId,
            Age18_24 = audience.Age18_24,
            Age25_34 = audience.Age25_34,
            Age35_44 = audience.Age35_44,
            MalePercent = audience.MalePercent,
            FemalePercent = audience.FemalePercent,
            TopCountries = audience.TopCountries,
            TopCities = audience.TopCities,
            Interests = audience.Interests,
            FakeFollowersPercent = audience.FakeFollowersPercent
        };
    }
}
