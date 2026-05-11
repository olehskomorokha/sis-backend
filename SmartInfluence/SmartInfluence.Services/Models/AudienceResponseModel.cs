namespace SmartInfluence.Services.Models;

public class AudienceResponseModel
{
    public int Id { get; set; }
    public int InfluencerId { get; set; }
    public decimal Age18_24 { get; set; }
    public decimal Age25_34 { get; set; }
    public decimal Age35_44 { get; set; }
    public decimal MalePercent { get; set; }
    public decimal FemalePercent { get; set; }
    public string TopCountries { get; set; } = string.Empty;
    public string TopCities { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public decimal FakeFollowersPercent { get; set; }
}
