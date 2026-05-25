namespace SmartInfluence.Services.Models;

public class InfluencerRecommendationFiltersModel
{
    public string Description { get; set; }
    public string? Country { get; set; }
    public List<string> Tags { get; set; } = [];
    public int? MinFollowersCount { get; set; }
    public int? MinAvgViews { get; set; }
}
