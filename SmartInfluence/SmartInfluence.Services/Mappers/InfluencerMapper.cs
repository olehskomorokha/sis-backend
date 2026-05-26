using Elastic.Clients.Elasticsearch.MachineLearning;
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
        };
    }

    public static InfluencerScoreModel MapToScoreModel(RecommendedChannelModel model)
    {
        return new InfluencerScoreModel
        {
            EngagementRate = model.EngagementRate,
            BrandFitScore = 0,
            AvgViews = model.AvgView,
            AvgComments = model.AvgComment,
            AvgLikes = model.AvgLike,
        };
    }
    public static RecommendedChannelModel MapToRecommendedChannelModel(
        YouTubeApi.UkrainianYouTubeBloggerDto channel,
        double? score,
        string? aiReview = null)
    {
        return new RecommendedChannelModel()
        {
            score = score,
            ChannelName = channel.Name,
            ChannelId =  channel.ChannelId,
            ChannelUrl = channel.ChannelUrl,
            CountryCode = channel.CountryCode ?? string.Empty,
            Description = channel.Description ?? string.Empty,
            AvatarUrl = channel.AvatarUrl ?? string.Empty,
            EngagementRate = channel.Statictics.EngagementRate,
            VideoCount = channel.Statictics.PerHalfYear.VideoCount,
            PostPerDay = channel.Statictics.PerHalfYear.PostPerDay,
            AvgLike = channel.Statictics.PerHalfYear.AvgLike,
            AvgView = channel.Statictics.PerHalfYear.AvgView,
            AvgComment = channel.Statictics.PerHalfYear.AvgComment,
            AiReview = aiReview
        };
    }
}
