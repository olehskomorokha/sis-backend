using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    readonly IConfiguration _configuration;
    
    public ClientService(IClientRepository clientRepository, IConfiguration configuration)
    {
        _clientRepository = clientRepository;
        _configuration = configuration;
    }

    public async Task CreateAsync(CreateClientModel model)
    {
        await _clientRepository.CreateAsync(ClientMapper.MapToCreateClientModel(model));
        
    }
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }
    public string GetAccessToken(int userId, string email)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email), 
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(60)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}