namespace SmartInfluence.Data.Entities;

public class ClientInfluencer
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int InfluencerId { get; set; }
    public int TotalScore { get; set; }
    
    public decimal BrandFitScore { get; set; }
    
    public string AiReview { get; set; }
    public Status Status { get; set; }
    public decimal PredictedEngagement { get; set; }
    public Client? Client { get; set; }
    public Influencers? Influencer { get; set; }
}

public enum Status
{
    Active,
    Inactive,
}
