namespace SmartInfluence.Services.Models;

public class InfluencerResponseModel
{
    public int Id { get; set; }
    public string Platform { get; set; }
    public string Description { get; set; }
    public string Country { get; set; }
    public string Lenguage { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}
