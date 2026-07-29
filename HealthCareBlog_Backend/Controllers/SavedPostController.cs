using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

[Route("api/saved-posts")]
[ApiController]
[Authorize]
public class SavedPostController : ControllerBase
{
    private readonly ISavedPostService _savedPostService;

    public SavedPostController(ISavedPostService savedPostService)
    {
        _savedPostService = savedPostService;
    }

    [HttpPost("{postId:int}")]
    public async Task<ActionResult> SavePost(int postId)
    {
        var userId = User.GetUserId()!;
        await _savedPostService.SavePostAsync(userId, postId);
        return Ok(new { message = "Post saved successfully.", success = true });
    }

    [HttpDelete("{postId:int}")]
    public async Task<ActionResult> UnsavePost(int postId)
    {
        var userId = User.GetUserId()!;
        await _savedPostService.UnsavePostAsync(userId, postId);
        return Ok(new { message = "Post unsaved successfully.", success = true });
    }

    [HttpGet]
    public async Task<ActionResult<List<ViewPostDTO>>> GetSavedPosts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId()!;
        var posts = await _savedPostService.GetSavedPostsAsync(userId, page, pageSize);
        return Ok(posts);
    }
}
