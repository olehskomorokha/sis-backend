using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;
using SmartInfluence.Services.Services;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ElasticsearchController : ControllerBase
{
    private readonly IElasticsearchService _elasticserchService;

    public ElasticsearchController(IElasticsearchService elasticserchService)
    {
        _elasticserchService = elasticserchService;
    }

    [HttpGet("GetById/{id}")]
    public async Task<object> GetById(string id)
    {
        return await _elasticserchService.GetById(id);
    }
    [HttpGet("bloggerTags")]
    public async Task<List<string>> GetAllBloggerTags()
    {
        return await _elasticserchService.GetAllBloggerTags();
    }
    
    [HttpGet("VideoDetailsByChannel/{channelId}")]
    public async Task<List<VideoDetailModel>> GetAllVideoDetailsByChannel(string channelId)
    {
        return await _elasticserchService.GetAllVideoDetailsByChannel(channelId);
    }

    [HttpPost("filters")]
    public async Task<List<InfluencerSearchModel>> GetByFilters([FromBody] FiltersModel model)
    {
        return await _elasticserchService.GetByFilters(model);
    }
    
}