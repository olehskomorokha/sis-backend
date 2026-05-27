namespace SmartInfluence.Data.Entities;

public class Influencers
{
    public int Id { get; set; }
    public string InfluencerId { get; set; }
    public string ChannelName { get; set; }
    public string Platform { get; set; }
    public string Description { get; set; }
    public string Country { get; set; }
    public string Lenguage { get; set; }
    public string AvatarUrl { get; set; }
    public string AiReview { get; set; }
    public int FollowersCount { get; set; }
}
