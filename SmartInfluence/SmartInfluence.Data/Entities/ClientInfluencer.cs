namespace SmartInfluence.Data.Entities;

public class ClientInfluencer
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int InfluencerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PredictedReach { get; set; }
    public decimal PredictedEngagement { get; set; }

    public Client? Campaign { get; set; }
    public Influencers? Influencer { get; set; }
}
