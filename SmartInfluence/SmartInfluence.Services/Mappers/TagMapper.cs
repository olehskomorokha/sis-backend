using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class TagMapper
{
    public static TagResponseModel MapToResponseModel(Tag tag)
    {
        return new TagResponseModel
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}
