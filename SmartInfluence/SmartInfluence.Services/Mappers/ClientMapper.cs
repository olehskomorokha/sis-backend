using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class ClientMapper
{
    public static ClientResponseModel MapToResponseModel(Client client)
    {
        return new ClientResponseModel
        {
            Id = client.Id,
            Brand = client.Brand,
            Email = client.Email,
            TargetCountry = client.TargetCountry,
            CreatedAt = client.CreatedAt
        };
    }

    public static Client MapToCreateClientModel(CreateClientModel client)
    {
        return new Client()
        {
            Brand = client.Brand,
            Email = client.Email,
            Password = client.Password,
            CreatedAt = DateTime.UtcNow
        };
    }
}
