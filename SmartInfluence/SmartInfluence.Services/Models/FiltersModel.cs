namespace SmartInfluence.Services.Models;

public class FiltersModel
{
    public List<string>? Tags { get; set; } = [];
    public string? Country { get; set; }
    public int? MinFollowersCount { get; set; }
    public int? MaxFollowersCount { get; set; }
    public int? ResponseCount { get; set; }
    
}