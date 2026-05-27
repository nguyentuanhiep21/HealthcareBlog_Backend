using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpPost("{followingId}")]
    public async Task<ActionResult> FollowUser(string followingId)
    {
        var followerId = User.GetUserId()!;
        await _followService.FollowUserAsync(followerId, followingId);
        return Ok(new { message = "Followed successfully.", success = true });
    }

    [HttpDelete("{followingId}")]
    public async Task<ActionResult> UnfollowUser(string followingId)
    {
        var followerId = User.GetUserId()!;
        await _followService.UnfollowUserAsync(followerId, followingId);
        return Ok(new { message = "Unfollowed successfully.", success = true });
    }

    [HttpGet("{userId}/following-users")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FollowUserDTO>>> GetFollowingUsers(
        string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var currentUserId = User.GetUserId();
        var users = await _followService.GetFollowingUsersAsync(userId, currentUserId, page, pageSize);
        return Ok(users);
    }
}
