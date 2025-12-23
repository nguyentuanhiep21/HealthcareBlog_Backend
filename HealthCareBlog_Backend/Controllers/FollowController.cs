using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers
{
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
            var followerId = User.GetUserId();
            if (string.IsNullOrEmpty(followerId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _followService.FollowUserAsync(followerId, followingId);
            return Ok(new { message = "Followed successfully.", success = result });
        }

        [HttpDelete("{followingId}")]
        public async Task<ActionResult> UnfollowUser(string followingId)
        {
            var followerId = User.GetUserId();
            if (string.IsNullOrEmpty(followerId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _followService.UnfollowUserAsync(followerId, followingId);
            return Ok(new { message = "Unfollowed successfully.", success = result });
        }

        [HttpGet("{userId}/followers")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetFollowers(
            string userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            var followers = await _followService.GetFollowersAsync(userId, page, pageSize);
            return Ok(followers);
        }

        [HttpGet("{userId}/following")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetFollowing(
            string userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            var following = await _followService.GetFollowingAsync(userId, page, pageSize);
            return Ok(following);
        }

        [HttpGet("{userId}/is-following")]
        public async Task<ActionResult<bool>> IsFollowing(string userId)
        {
            var followerId = User.GetUserId();
            if (string.IsNullOrEmpty(followerId))
            {
                return Unauthorized("User not authenticated.");
            }

            var isFollowing = await _followService.IsFollowingAsync(followerId, userId);
            return Ok(new { isFollowing });
        }

        [HttpGet("{userId}/following-users")]
        [AllowAnonymous]
        public async Task<ActionResult<List<FollowUserDTO>>> GetFollowingUsers(
            string userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            var currentUserId = User.GetUserId();
            var followingUsers = await _followService.GetFollowingUsersAsync(userId, currentUserId, page, pageSize);
            return Ok(followingUsers);
        }

        [HttpGet("{userId}/followers-users")]
        [AllowAnonymous]
        public async Task<ActionResult<List<FollowUserDTO>>> GetFollowersUsers(
            string userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            var currentUserId = User.GetUserId();
            var followers = await _followService.GetFollowersUsersAsync(userId, currentUserId, page, pageSize);
            return Ok(followers);
        }
    }
}
