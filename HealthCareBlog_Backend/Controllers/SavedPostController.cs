using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SavedPostController : ControllerBase
    {
        private readonly ISavedPostService _savedPostService;

        public SavedPostController(ISavedPostService savedPostService)
        {
            _savedPostService = savedPostService;
        }

        [HttpPost("{postId}")]
        public async Task<ActionResult> SavePost(int postId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _savedPostService.SavePostAsync(userId, postId);
            return Ok(new { message = "Post saved successfully.", success = result });
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> UnsavePost(int postId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _savedPostService.UnsavePostAsync(userId, postId);
            return Ok(new { message = "Post unsaved successfully.", success = result });
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewPostDTO>>> GetSavedPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var posts = await _savedPostService.GetSavedPostsAsync(userId, page, pageSize);
            return Ok(posts);
        }

    }
}
