namespace SmartInfluence.Data.Entities;

public class Campaign
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string TargetCountry { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string Goals { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
