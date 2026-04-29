using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateClientModel model)
    {
        await _clientService.CreateAsync(model);
        return Ok();
    }
}
