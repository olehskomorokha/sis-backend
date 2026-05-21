namespace SmartInfluence.Services.Models;

public class ProductCriteriaModel
{
    public string ProductCategory { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = [];
    public List<string> Platforms { get; set; } = [];
    public string TargetCountry { get; set; } = string.Empty;
    public string TargetGender { get; set; } = string.Empty;
    public int? AgeMin { get; set; }
    public int? AgeMax { get; set; }
    public decimal? MaxFakeFollowersPercent { get; set; }
}
