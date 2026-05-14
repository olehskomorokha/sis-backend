namespace SmartInfluence.Services.Models;

public class RecommendedInfluencerModel
{
    public int InfluencerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}
