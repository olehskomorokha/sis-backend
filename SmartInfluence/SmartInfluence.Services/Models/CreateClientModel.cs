namespace SmartInfluence.Services.Models;

public class CreateClientModel
{
    public string Brand { get; set; } = string.Empty;
    public string Email { get; set; }
    public string Password { get; set; } = string.Empty;
}