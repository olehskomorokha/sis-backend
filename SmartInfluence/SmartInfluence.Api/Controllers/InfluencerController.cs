using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfluencerController : ControllerBase
{
    private readonly IInfluencerService _influencerService;

    public InfluencerController(IInfluencerService influencerService)
    {
        _influencerService = influencerService;
    }

    [HttpGet("clientInfluencers/{clientId:int}")]
    public async Task<ActionResult<List<InfluencerResponseModel>>> GetByClientIdAsync(int clientId)
    {
        var influencers = await _influencerService.GetByClientIdAsync(clientId);
        return Ok(influencers);
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

        return Ok(await _influencerService.RecommendAsync(request));
    }

    [HttpPost("add-influencer/{clientId}")]
    [Authorize]
    public async Task<ActionResult<InfluencerResponseModel>> SaveRecommendedAsync(
        [FromBody] RecommendedChannelModel request, int clientId)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ChannelId) || string.IsNullOrWhiteSpace(request.ChannelName))
        {
            return BadRequest("ChannelId and ChannelName are required.");
        }

        var influencer = await _influencerService.SaveRecommendedAsync(request, clientId);
        return CreatedAtAction("GetById", new { id = influencer.Id }, influencer);
    }
    
}
