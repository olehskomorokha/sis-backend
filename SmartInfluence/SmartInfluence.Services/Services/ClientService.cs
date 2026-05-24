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
    private readonly IInfluencerRepository _influencerRepository;
    private readonly IConfiguration _configuration;

    public ClientService(
        IClientRepository clientRepository,
        IInfluencerRepository influencerRepository,
        IConfiguration configuration)
    {
        _clientRepository = clientRepository;
        _influencerRepository = influencerRepository;
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

    public async Task<bool> UpdateAsync(int id, UpdateClientModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
        {
            return false;
        }

        ApplyClientSettings(client, model);
        await _clientRepository.UpdateAsync(client);
        return true;
    }

    public async Task<List<InfluencerResponseModel>> GetInfluencersByClientIdAsync(int clientId)
    {
        var influencers = await _influencerRepository.GetByClientIdAsync(clientId);
        return influencers.Select(InfluencerMapper.MapToResponseModel).ToList();
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

    private static void ApplyClientSettings(
        Data.Entities.Client client,
        UpdateClientModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Brand))
        {
            client.Brand = model.Brand;
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            client.Email = model.Email;
        }

        if (model.Budget.HasValue)
        {
            client.Budget = model.Budget;
        }

        if (!string.IsNullOrWhiteSpace(model.TargetCountry))
        {
            client.TargetCountry = model.TargetCountry;
        }

        if (!string.IsNullOrWhiteSpace(model.TargetAudience))
        {
            client.TargetAudience = model.TargetAudience;
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
