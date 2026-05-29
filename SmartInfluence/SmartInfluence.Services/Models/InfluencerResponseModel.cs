namespace SmartInfluence.Services.Models;

public class InfluencerResponseModel
{
    public int Id { get; set; }
    public string Platform { get; set; }
    public string ChannelName { get; set; }
    public string ChannelUrl { get; set; }
    public string Description { get; set; }
    public string Country { get; set; }
    public string AvatarUrl { get; set; }
    public string? AiReview { get; set; }
    public int FollowersCount { get; set; }
    public InfluencerScoreModel? Score { get; set; }
}
