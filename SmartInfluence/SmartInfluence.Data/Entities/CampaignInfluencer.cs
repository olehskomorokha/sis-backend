namespace SmartInfluence.Data.Entities;

public class CampaignInfluencer
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int InfluencerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PredictedReach { get; set; }
    public decimal PredictedEngagement { get; set; }

    public Campaign? Campaign { get; set; }
    public Influencers? Influencer { get; set; }
}
