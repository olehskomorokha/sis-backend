namespace SmartInfluence.Services.Models;

public class InfluencerRecommendationRequestModel
{
    public string ProductDescription { get; set; } = string.Empty;
    public int Limit { get; set; } = 3;
    public InfluencerRecommendationFiltersModel Filters { get; set; } = new();
}
