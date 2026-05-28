using Elastic.Clients.Elasticsearch.MachineLearning;
using SmartInfluence.Collector.YouTube;
using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class InfluencerMapper
{
    public static InfluencerResponseModel MapToResponseModel(
        Influencers influencer,
        InfluencerScore? score = null)
    {
        return new InfluencerResponseModel
        {
            Id = influencer.Id,
            Platform = influencer.Platform,
            ChannelUrl = influencer.ChannelUrl,
            AvatarUrl = influencer.AvatarUrl,
            AiReview =  influencer.AiReview,
            ChannelName = influencer.ChannelName,
            Description = influencer.Description,
            Country = influencer.Country,
            FollowersCount = influencer.FollowersCount,
            Score = score == null ? null : MapToScoreModel(score)
        };
    }

    public static InfluencerScoreModel MapToScoreModel(InfluencerScore score)
    {
        return new InfluencerScoreModel
        {
            EngagementRate = Convert.ToSingle(score.EngagementScore),
            BrandFitScore = score.BrandFitScore,
            AvgViews = score.AvgViews,
            AvgLikes = score.AvgLikes,
            AvgComments = score.AvgComments,
            CalculatedAt = score.CalculatedAt
        };
    }
    
    public static RecommendedChannelModel MapToRecommendedChannelModel(
        YouTubeApi.UkrainianYouTubeBloggerDto channel,
        double? score,
        string? aiReview = null)
    {
        return new RecommendedChannelModel()
        {
            Score = score,
            ChannelName = channel.Name,
            ChannelId =  channel.ChannelId,
            ChannelUrl = channel.ChannelUrl,
            CountryCode = channel.CountryCode ?? string.Empty,
            Language = channel.Language ?? string.Empty,
            Description = channel.Description ?? string.Empty,
            AvatarUrl = channel.AvatarUrl ?? string.Empty,
            FollowersCount = channel.Fields.SubscriberCount ?? 0,
            EngagementRate = channel.Statictics.EngagementRate,
            VideoCount = channel.Statictics.PerHalfYear.VideoCount,
            PostPerDay = channel.Statictics.PerHalfYear.PostPerDay,
            AvgLike = channel.Statictics.PerHalfYear.AvgLike,
            AvgView = channel.Statictics.PerHalfYear.AvgView,
            AvgComment = channel.Statictics.PerHalfYear.AvgComment,
            AiReview = aiReview
        };
    }

    public static Influencers MapToInfluencerEntity(RecommendedChannelModel model)
    {
        return new Influencers
        {
            InfluencerId = model.ChannelId ?? string.Empty,
            ChannelName = model.ChannelName,
            ChannelUrl = model.ChannelUrl,
            Platform = "YouTube",
            Description = model.Description,
            Country = model.CountryCode,
            Lenguage = model.Language,
            AvatarUrl = model.AvatarUrl ?? string.Empty,
            AiReview = model.AiReview,
            FollowersCount = model.FollowersCount
        };
    }

    public static ClientInfluencer MapToClientInfluencer(int clientId, int influencerId)
    {
        return new ClientInfluencer
            {
                ClientId = clientId,
                InfluencerId = influencerId,
                Status = Status.Active,
                PredictedEngagement = CalculatePredictedEngagement(23),
            };
    }

    public static InfluencerScore MapToInfluencerScoreEntity(RecommendedChannelModel model, int influencerId)
    {
        return new InfluencerScore
        {
            InfluencerId = influencerId,
            EngagementScore = Convert.ToDecimal(model.EngagementRate),
            BrandFitScore = 0,
            AvgViews = model.AvgView,
            AvgLikes = model.AvgLike,
            AvgComments = model.AvgComment,
            PostsCount = model.VideoCount,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public static int CalculatePredictedEngagement(int i)
    {
        return i;
    }
}
