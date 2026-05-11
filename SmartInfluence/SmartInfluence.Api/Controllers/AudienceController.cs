using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AudienceController : ControllerBase
{
    private readonly IAudienceService _audienceService;

    public AudienceController(IAudienceService audienceService)
    {
        _audienceService = audienceService;
    }

    [HttpGet("influencer/{influencerId:int}")]
    public async Task<ActionResult<AudienceResponseModel>> GetByInfluencerIdAsync(int influencerId)
    {
        var audience = await _audienceService.GetByInfluencerIdAsync(influencerId);
        if (audience == null)
        {
            return NotFound();
        }

        return Ok(audience);
    }
}
