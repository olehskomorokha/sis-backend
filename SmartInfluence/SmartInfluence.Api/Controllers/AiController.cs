using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("/{descriprion}")]
    public async Task<ActionResult<ProductCriteriaModel>> GenerateTagsByProductDescription(string descriprion)
    {
        var criteriamodel = await _productQueryAiService.ParseProductDescriptionAsync(descriprion);
        return Ok(criteriamodel);
    }

    [HttpGet("rew/{channelId}")]
    public async Task<ActionResult<string>> ChannelReview(string channelId)
    {
        return await _productQueryAiService.AiChannelReviewAsync(channelId);
    }
    
    
    
}
