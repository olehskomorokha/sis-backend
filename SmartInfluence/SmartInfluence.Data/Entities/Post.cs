namespace SmartInfluence.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public string PlatformPostId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Hashtags { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public DateTime PostedAt { get; set; }

    public Influencers? Influencer { get; set; }
}
