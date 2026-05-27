namespace SmartInfluence.Services.Models;

public class ClientResponseModel
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? TargetCountry { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
