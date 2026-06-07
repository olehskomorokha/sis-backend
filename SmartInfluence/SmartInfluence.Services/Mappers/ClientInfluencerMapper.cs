using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;
namespace SmartInfluence.Services.Mappers;

public class ClientInfluencerMapper
{
    public static ClientInfluencerModel MapToClientInfluencerCardModel(
        ClientInfluencer clientInfluencer,
        Influencers influencers,
        InfluencerScore? score = null)
    {
        return new ClientInfluencerModel
        {
            Id = clientInfluencer.Id,
            InfluenceId = influencers.Id,
            ChannelId = influencers.InfluencerId,
            ChannelName = influencers.ChannelName,
            ChannelUrl = influencers.ChannelUrl,
            Platform = influencers.Platform,
            Description = influencers.Description,
            Country = influencers.Country,
            Lenguage = influencers.Lenguage,
            AvatarUrl = influencers.AvatarUrl,
            FollowersCount = influencers.FollowersCount,
            TotalScore = clientInfluencer.TotalScore,
            BrandFitScore = clientInfluencer.BrandFitScore,
            AiReview = clientInfluencer.AiReview,
            Status = clientInfluencer.Status,
            PredictedEngagement = clientInfluencer.PredictedEngagement,
            InfluencerScore = score == null ? null : InfluencerMapper.MapToScoreModel(score)
        };
    }
    public static ClientInfluencer MapToClientInfluencer(int clientId, RecommendedChannelModel model, int influencerId)
    {
        return new ClientInfluencer
        {
            ClientId = clientId,
            InfluencerId = influencerId,
            BrandFitScore = model.BrandFitScore,
            Status = Status.Active,
            TotalScore = model.TotalScore,
            AiReview = model.AiReview ?? "",
            PredictedEngagement = model.PredictedEngagement,
        };
    }
}
