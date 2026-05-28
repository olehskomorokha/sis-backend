using SmartInfluence.Data.Entities;

namespace SmartInfluence.Services.Models;

public class ClientInfluencerModel
{
    public int Id { get; set; }
    public string InfluencerId { get; set; }
    public string ChannelName { get; set; }
    public string ChannelUrl { get; set; }
    public string Platform { get; set; }
    public string Description { get; set; }
    public string Country { get; set; }
    public string Lenguage { get; set; }
    public string AvatarUrl { get; set; }
    public int FollowersCount { get; set; }
    public int TotalScore { get; set; }
    
    public decimal BrandFitScore { get; set; }
    
    public string AiReview { get; set; }
    public Status Status { get; set; }
    public decimal PredictedEngagement { get; set; }
    public InfluencerScoreModel? InfluencerScore { get; set; }
    
    
}