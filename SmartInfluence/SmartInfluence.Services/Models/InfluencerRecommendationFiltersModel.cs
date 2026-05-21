namespace SmartInfluence.Services.Models;

public class InfluencerRecommendationFiltersModel
{
    public string? Platform { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public int? MinFollowersCount { get; set; }
    public int? MaxFollowersCount { get; set; }
    public decimal? MinEngagementRatePercent { get; set; }
}
