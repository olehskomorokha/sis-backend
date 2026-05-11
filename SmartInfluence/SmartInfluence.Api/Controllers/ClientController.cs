using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Exceptions;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientResponseModel>>> GetAllAsync()
    {
        return Ok(await _clientService.GetAllAsync());
    }

    [Authorize]
    [HttpGet("influencers")]
    public async Task<ActionResult<List<InfluencerResponseModel>>> GetInfluencersAsync()
    {
        var clientIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(clientIdValue, out var clientId))
        {
            return Unauthorized();
        }

        return Ok(await _clientService.GetInfluencersByClientIdAsync(clientId));
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateClientModel model)
    {
        await _clientService.CreateAsync(model);
        return Ok();
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync([FromBody] UpdateClientModel model)
    {
        var clientIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(clientIdValue, out var clientId))
        {
            return Unauthorized();
        }

        var isUpdated = await _clientService.UpdateAsync(clientId, model);
        if (!isUpdated)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginClientModel model)
    {
        try
        {
            var accessToken = await _clientService.LoginAsync(model);
            return Ok(new { accessToken });
        }
        catch (LoginException ex)
        {
            return Unauthorized(new { code = ex.Code, message = ex.Message });
        }
    }
}
