using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfluencerController : ControllerBase
{
    private readonly IInfluencerService _influencerService;
    private readonly IInfluencerRecommendationService _influencerRecommendationService;

    public InfluencerController(
        IInfluencerService influencerService,
        IInfluencerRecommendationService influencerRecommendationService
        )
    {
        _influencerService = influencerService;
        _influencerRecommendationService = influencerRecommendationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InfluencerResponseModel>>> GetAllAsync()
    {
        return Ok(await _influencerService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InfluencerResponseModel>> GetByIdAsync(int id)
    {
        var influencer = await _influencerService.GetByIdAsync(id);
        if (influencer == null)
        {
            return NotFound();
        }

        return Ok(influencer);
    }

    [HttpPost("recommendations")]
    public async Task<ActionResult<ElasticInfluencerRecommendationResponseModel>> RecommendAsync(
        [FromBody] InfluencerRecommendationFiltersModel request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Description is required.");
        }

        return Ok(await _influencerRecommendationService.RecommendAsync(request));
    }
}
