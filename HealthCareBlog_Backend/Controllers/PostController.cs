using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IReportService _reportService;

        public PostController(IPostService postService, IReportService reportService)
        {
            _postService = postService;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewPostDTO>>> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            var posts = await _postService.ViewPostAsync(userId, page, pageSize);
            return Ok(posts);
        }

        [HttpPost("{userId}")]
        [Authorize]
        public async Task<ActionResult<PostDetailDTO>> CreatePost([FromBody] CreatePostDTO createPostDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var post = await _postService.CreatePostAsync(userId, createPostDTO);
            return Ok(post);
        }

        [HttpPut("{postId}")]
        [Authorize]
        public async Task<ActionResult<PostDetailDTO>> UpdatePost(int postId, [FromBody] UpdatePostDTO updatePostDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var post = await _postService.UpdatePostAsync(userId, postId, updatePostDTO);
            return Ok(post);
        }

        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<ActionResult> DeletePost(int postId)
        {
            var result = await _postService.DeletePostAsync(postId);
            return Ok(new { message = "Post deleted successfully." });
        }

        [HttpPost("{postId}/like")]
        [Authorize]
        public async Task<ActionResult> LikePost(int postId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _postService.LikePostAsync(userId, postId);
            return Ok(new { message = "Post liked successfully." });
        }

        [HttpDelete("{postId}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikePost(int postId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _postService.UnlikePostAsync(userId, postId);
            return Ok(new { message = "Post unliked successfully." });
        }

        [HttpPost("{postId}/report")]
        [Authorize]
        public async Task<ActionResult<ViewReportDTO>> ReportPost(int postId, [FromBody] CreateReportDTO createReportDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            createReportDTO.ContentType = "Post";
            createReportDTO.ContentId = postId.ToString();

            var report = await _reportService.CreateReportAsync(userId, createReportDTO);
            return Ok(new { message = "Post reported successfully.", data = report });
        }
    }
}
