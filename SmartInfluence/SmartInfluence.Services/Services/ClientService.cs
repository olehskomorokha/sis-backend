using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Exceptions;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IConfiguration _configuration;

    public ClientService(IClientRepository clientRepository, IConfiguration configuration)
    {
        _clientRepository = clientRepository;
        _configuration = configuration;
    }

    public async Task<List<ClientResponseModel>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        return clients.Select(ClientMapper.MapToResponseModel).ToList();
    }
    
    public async Task CreateAsync(CreateClientModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.Password = HashPassword(model.Password);
        await _clientRepository.CreateAsync(ClientMapper.MapToCreateClientModel(model));
    }

    public async Task<string> LoginAsync(LoginClientModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var user = await _clientRepository.GetByEmailAsync(model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            throw new LoginException("invalid_credentials", "Invalid email or password.");
        }

        return GetAccessToken(user.Id, user.Email);
    }

    private string HashPassword(string password)
    {
        try
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
        catch (Exception)
        {
            throw new PasswordHashingException("failed_to_hash", "Failed to hash client password.");
        }
    }

    private string GetAccessToken(int userId, string email)
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
            expires: DateTime.UtcNow.AddDays(60),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
