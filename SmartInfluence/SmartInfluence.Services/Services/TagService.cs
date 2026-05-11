using SmartInfluence.Data.Interfaces;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<TagResponseModel>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(TagMapper.MapToResponseModel).ToList();
    }
}
