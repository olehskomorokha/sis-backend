using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Interfaces;

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
    public async Task<ActionResult<List<ClientInfluencer>>> GetAsync(int clientId)
    {
        return Ok(await _clientInfluencerService.GetAllAsync(clientId));
    }
}