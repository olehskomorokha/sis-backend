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

    [HttpGet]
    public async Task<List<string>> GetAllBloggerTags()
    {
        return await _elasticserchService.GetAllBloggerTags();
    }

    [HttpPost("filters")]
    public async Task<List<InfluencerSearchModel>> GetByFilters([FromBody] FiltersModel model)
    {
        return await _elasticserchService.GetByFilters(model);
    }
}