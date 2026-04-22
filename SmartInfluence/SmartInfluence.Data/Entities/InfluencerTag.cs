namespace SmartInfluence.Data.Entities;

public class InfluencerTag
{
    public int InfluencerId { get; set; }
    public int TagId { get; set; }

    public Influencers? Influencer { get; set; }
    public Tag? Tag { get; set; }
}
