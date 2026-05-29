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
            BrandFitScore = 23,
            Status = Status.Active,
            TotalScore = 24,
            AiReview = model.AiReview ?? "",
            PredictedEngagement = CalculatePredictedEngagement(23),
        };
    }
    
    
    public static int CalculatePredictedEngagement(int i)
    {
        return i;
    }
    
}
