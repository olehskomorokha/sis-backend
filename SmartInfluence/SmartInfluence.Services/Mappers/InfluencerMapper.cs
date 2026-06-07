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
            EngagementRate = Convert.ToSingle(score.EngagementRate),
            AvgViews = score.AvgViews,
            AvgLikes = score.AvgLikes,
            PostCount = score.PostsCount,
            AvgComments = score.AvgComments,
            CalculatedAt = score.CalculatedAt
        };
    }
    
    public static RecommendedChannelModel MapToRecommendedChannelModel(
        YouTubeBloggerDocument channel,
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
            AvgComment = channel.Statictics.PerHalfYear.AvgComment
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
            AvatarUrl = model.AvatarUrl,
            FollowersCount = model.FollowersCount
        };
    }

    

    public static InfluencerScore MapToInfluencerScoreEntity(RecommendedChannelModel model, int influencerId)
    {
        return new InfluencerScore
        {
            InfluencerId = influencerId,
            EngagementRate = Convert.ToDecimal(model.EngagementRate),
            AvgViews = model.AvgView,
            AvgLikes = model.AvgLike,
            AvgComments = model.AvgComment,
            PostsCount = model.VideoCount,
            CalculatedAt = DateTime.UtcNow
        };
    }

    
}

