using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientInfluencerController : ControllerBase
{
    private readonly IClientInfluencerService _clientInfluencerService;

    public ClientInfluencerController(IClientInfluencerService clientInfluencerService)
    {
        _clientInfluencerService = clientInfluencerService;
    }

    [HttpGet("{clientId}")]
    public async Task<ActionResult<List<ClientInfluencerModel>>> GetAsync(int clientId)
    {
        return Ok(await _clientInfluencerService.GetAllAsync(clientId));
    }

    [Authorize]
    [HttpPut("{influencerId:int}")]
    public async Task<ActionResult> UpdateAsync(int influencerId, [FromBody] UpdateClientInfluencerModel model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        var clientIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(clientIdValue, out var clientId))
        {
            return Unauthorized();
        }

        var updated = await _clientInfluencerService.UpdateAsync(clientId, influencerId, model);
        if (!updated)
        {
            return NotFound();
        }

        return Ok();
    }

    [Authorize]
    [HttpDelete("{influencerId:int}")]
    public async Task<ActionResult> DeleteAsync(int influencerId)
    {
        var clientIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(clientIdValue, out var clientId))
        {
            return Unauthorized();
        }

        var deleted = await _clientInfluencerService.DeleteAsync(clientId, influencerId);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
