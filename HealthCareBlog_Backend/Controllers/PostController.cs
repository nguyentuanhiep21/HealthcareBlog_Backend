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

        [HttpGet("trending")]
        public async Task<ActionResult<List<ViewPostDTO>>> GetTrendingPosts()
        {
            try
            {
                var userId = User.GetUserId();
                var trendingPosts = await _postService.GetTrendingPostsAsync(userId);
                return Ok(trendingPosts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] GetTrendingPosts Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
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
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _postService.DeletePostAsync(postId);
                return Ok(new { message = "Xóa bài viết thành công.", success = true });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (UnauthorizedException ex)
            {
                return StatusCode(403, new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] DeletePost Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
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
        public async Task<ActionResult<ViewReportDTO>> ReportPost(int postId, [FromBody] CreatePostReportDTO createPostReportDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Bạn chưa đăng nhập.", success = false });
                }

                var createReportDTO = new CreateReportDTO
                {
                    ContentType = "Post",
                    ContentId = postId.ToString(),
                    Reason = createPostReportDTO.Reason,
                    Description = createPostReportDTO.Description
                };

                var (report, isExisting) = await _reportService.CreateReportAsync(userId, createReportDTO);
                
                var message = isExisting 
                    ? "Bạn đã báo cáo bài viết này trước đó rồi." 
                    : "Báo cáo bài viết thành công.";
                
                return Ok(new { message, success = true, data = report });
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
                Console.WriteLine($"Error reporting post: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ViewPostDTO>>> GetAllPostsForAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchQuery = null)
        {
            try
            {
                var userId = User.GetUserId();
                var posts = await _postService.GetAllPostsForAdminAsync(userId, page, pageSize, searchQuery);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] GetAllPostsForAdmin Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete("admin/{postId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminDeletePost(int postId)
        {
            try
            {
                var result = await _postService.DeletePostAsync(postId);
                return Ok(new { message = "Xóa bài viết thành công.", success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostController] AdminDeletePost Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }
    }
}
