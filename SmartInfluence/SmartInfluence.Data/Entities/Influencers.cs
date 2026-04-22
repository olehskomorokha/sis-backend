namespace SmartInfluence.Data.Entities;

public class Influencers
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Lenguage { get; set; } = string.Empty;
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public decimal AvgViews { get; set; }
    public decimal AvgLikes { get; set; }
    public decimal AvgComments { get; set; }
}
