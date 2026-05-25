using SmartInfluence.Collector.YouTube;
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

    public static RecommendedChannelModel MapToRecommendedChannelModel(YouTubeApi.UkrainianYouTubeBloggerDto channel)
    {
        return new RecommendedChannelModel()
        {
            ChannelName = channel.Name,
            ChannelUrl = channel.ChannelUrl,
            CountryCode = channel.CountryCode ?? string.Empty,
            Description = channel.Description ?? string.Empty,
            AvatarUrl = channel.AvatarUrl ?? string.Empty,
            EngagementRate = channel.Statictics.EngagementRate,
            VideoCount = channel.Statictics.PerHalfYear.VideoCount,
            PostPerDay = channel.Statictics.PerHalfYear.PostPerDay,
            AvgLike = channel.Statictics.PerHalfYear.AvgLike,
            AvgView = channel.Statictics.PerHalfYear.AvgView,
            AvgComment = channel.Statictics.PerHalfYear.AvgComment
        };
    }
}
