using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Exceptions;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiController : ControllerBase
{
    private readonly IProductQueryAiService _productQueryAiService;

    public AiController(IProductQueryAiService productQueryAiService)
    {
        _productQueryAiService = productQueryAiService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductCriteriaModel>> GenerateTagsByProductDescription([FromBody] GenerateTagsModel description)
    {
        var criteriaModel = await _productQueryAiService.ParseProductDescriptionAsync(description.description);
        return Ok(criteriaModel);
    }

    [Authorize]
    [HttpGet("review/{channelId}")]
    public async Task<ActionResult<string>> ChannelReview(string channelId)
    {
        return await _productQueryAiService.AiChannelReviewAsync(channelId);
    }
}