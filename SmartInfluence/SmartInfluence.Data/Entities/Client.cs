namespace SmartInfluence.Data.Entities;

public class Client
{
    public int Id { get; set; }
    
    public string Brand { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public decimal? Budget { get; set; }
    public string? TargetCountry { get; set; }
    public string? TargetAudience { get; set; }
    public DateTime CreatedAt { get; set; }
}
