using System.ComponentModel.DataAnnotations;

namespace SmartInfluence.Data.Entities;

public class Client
{
    public int Id { get; set; } 
    [MaxLength(50)]
    public required string Brand { get; set; }
    
    [MaxLength(100)]
    public required string Email { get; set; }
    
    [MaxLength(100)]
    public required string Password { get; set; }
    public DateTime CreatedAt { get; set; }
}
