using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// PostController — chỉ xử lý HTTP request/response.
/// Không còn try/catch lặp lại — lỗi được GlobalExceptionHandlerMiddleware bắt tập trung.
/// </summary>
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
    public async Task<ActionResult<List<ViewPostDTO>>> GetPosts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var posts = await _postService.ViewPostAsync(userId, page, pageSize);
        return Ok(posts);
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<ViewPostDTO>>> GetTrendingPosts()
    {
        var userId = User.GetUserId();
        var posts = await _postService.GetTrendingPostsAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{postId:int}")]
    public async Task<ActionResult<PostDetailDTO>> GetPostById(int postId)
    {
        var userId = User.GetUserId();
        var post = await _postService.GetPostByIdAsync(userId, postId);
        return Ok(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDetailDTO>> CreatePost([FromBody] CreatePostDTO dto)
    {
        var userId = User.GetUserId()!;
        var post = await _postService.CreatePostAsync(userId, dto);
        return Ok(new { message = "Đăng bài thành công.", data = post, success = true });
    }

    [HttpPut("{postId:int}")]
    [Authorize]
    public async Task<ActionResult<PostDetailDTO>> UpdatePost(int postId, [FromBody] UpdatePostDTO dto)
    {
        var userId = User.GetUserId()!;
        var post = await _postService.UpdatePostAsync(userId, postId, dto);
        return Ok(new { message = "Cập nhật bài viết thành công.", data = post, success = true });
    }

    [HttpDelete("{postId:int}")]
    [Authorize]
    public async Task<ActionResult> DeletePost(int postId)
    {
        await _postService.DeletePostAsync(postId);
        return Ok(new { message = "Xóa bài viết thành công.", success = true });
    }

    [HttpPost("{postId:int}/like")]
    [Authorize]
    public async Task<ActionResult> LikePost(int postId)
    {
        var userId = User.GetUserId()!;
        await _postService.LikePostAsync(userId, postId);
        return Ok(new { message = "Thích bài viết thành công.", success = true });
    }

    [HttpDelete("{postId:int}/like")]
    [Authorize]
    public async Task<ActionResult> UnlikePost(int postId)
    {
        var userId = User.GetUserId()!;
        await _postService.UnlikePostAsync(userId, postId);
        return Ok(new { message = "Bỏ thích bài viết thành công.", success = true });
    }

    [HttpPost("{postId:int}/report")]
    [Authorize]
    public async Task<ActionResult<ViewReportDTO>> ReportPost(
        int postId, [FromBody] CreatePostReportDTO dto)
    {
        var userId = User.GetUserId()!;
        var createReportDTO = new CreateReportDTO
        {
            ContentType = "Post",
            ContentId = postId.ToString(),
            Reason = dto.Reason,
            Description = dto.Description
        };

        var (report, isExisting) = await _reportService.CreateReportAsync(userId, createReportDTO);
        var message = isExisting
            ? "Bạn đã báo cáo bài viết này trước đó rồi."
            : "Báo cáo bài viết thành công.";

        return Ok(new { message, success = true, data = report });
    }

    [HttpDelete("admin/{postId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AdminDeletePost(int postId)
    {
        await _postService.DeletePostAsync(postId);
        return Ok(new { message = "Xóa bài viết thành công.", success = true });
    }
}
