using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class InfluencerMapper
{
    public static InfluencerResponseModel MapToResponseModel(Influencers influencer)
    {
        return new InfluencerResponseModel
        {
            Id = influencer.Id,
            Platform = influencer.Platform,
            Description = influencer.Description,
            Country = influencer.Country,
            Lenguage = influencer.Lenguage,
            FollowersCount = influencer.FollowersCount,
            PostsCount = influencer.PostsCount,
            AvgViews = influencer.AvgViews,
            AvgLikes = influencer.AvgLikes,
            AvgComments = influencer.AvgComments,
            
        };
    }
}
