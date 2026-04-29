using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class ClientMapper
{
    public static Client MapToCreateClientModel(CreateClientModel client)
    {
        return new Client()
        {
            Brand = client.Brand,
            Email = client.Email,
            Password = client.Password,
            CreatedAt = new DateTime()
        };
    }
}