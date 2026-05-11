using Microsoft.AspNetCore.Mvc;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PostResponseModel>>> GetAllAsync()
    {
        return Ok(await _postService.GetAllAsync());
    }

    [HttpGet("influencer/{influencerId:int}")]
    public async Task<ActionResult<List<PostResponseModel>>> GetByInfluencerIdAsync(int influencerId)
    {
        return Ok(await _postService.GetByInfluencerIdAsync(influencerId));
    }
}
