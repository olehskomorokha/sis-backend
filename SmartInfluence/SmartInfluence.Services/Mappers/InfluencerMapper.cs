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
            UserName = influencer.UserName,
            FullName = influencer.FullName,
            Platform = influencer.Platform,
            Bio = influencer.Bio,
            Country = influencer.Country,
            Lenguage = influencer.Lenguage,
            FollowersCount = influencer.FollowersCount,
            FollowingCount = influencer.FollowingCount,
            PostsCount = influencer.PostsCount,
            AvgViews = influencer.AvgViews,
            AvgLikes = influencer.AvgLikes,
            AvgComments = influencer.AvgComments
        };
    }
}
