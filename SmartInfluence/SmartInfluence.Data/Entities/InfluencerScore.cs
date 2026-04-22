namespace SmartInfluence.Data.Entities;

public class InfluencerScore
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public decimal ScoreOverall { get; set; }
    public decimal ScoreEngagement { get; set; }
    public decimal ScoreAudienceQuality { get; set; }
    public decimal ScoreBrandFit { get; set; }
    public decimal ScoreAuthenticity { get; set; }
    public DateTime CalculatedAt { get; set; }

    public Influencers? Influencer { get; set; }
}
