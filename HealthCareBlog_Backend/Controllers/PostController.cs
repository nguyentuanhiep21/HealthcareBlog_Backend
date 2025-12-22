using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
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

        [HttpGet("{postId}")]
        public async Task<ActionResult<PostDetailDTO>> GetPostById(int postId)
        {
            try
            {
                var userId = User.GetUserId();
                var post = await _postService.GetPostByIdAsync(userId, postId);
                return Ok(post);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] GetPostById Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDetailDTO>> CreatePost([FromBody] CreatePostDTO createPostDTO)
        {
            try
            {
                Console.WriteLine("[PostController] CreatePost endpoint called");
                
                var userId = User.GetUserId();
                Console.WriteLine($"[PostController] User ID: {userId}");
                
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("[PostController] Error: User ID is null or empty");
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                Console.WriteLine("[PostController] Calling PostService.CreatePostAsync...");
                var post = await _postService.CreatePostAsync(userId, createPostDTO);
                
                Console.WriteLine("[PostController] Post created successfully");
                return Ok(new { message = "Đăng bài thành công.", data = post, success = true });
            }
            catch (BadRequestException ex)
            {
                Console.WriteLine($"[PostController] BadRequestException: {ex.Message}");
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine($"[PostController] NotFoundException: {ex.Message}");
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] Exception: {ex.GetType().Name}");
                Console.WriteLine($"[PostController] Message: {ex.Message}");
                Console.WriteLine($"[PostController] StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đăng bài. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPut("{postId}")]
        [Authorize]
        public async Task<ActionResult<PostDetailDTO>> UpdatePost(int postId, [FromBody] UpdatePostDTO updatePostDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var post = await _postService.UpdatePostAsync(userId, postId, updatePostDTO);
                return Ok(new { message = "Cập nhật bài viết thành công.", data = post, success = true });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (UnauthorizedException ex)
            {
                return StatusCode(403, new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] UpdatePost Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
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
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _postService.LikePostAsync(userId, postId);
                return Ok(new { message = "Thích bài viết thành công.", success = true });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] LikePost Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete("{postId}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikePost(int postId)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _postService.UnlikePostAsync(userId, postId);
                return Ok(new { message = "Bỏ thích bài viết thành công.", success = true });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] UnlikePost Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
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
