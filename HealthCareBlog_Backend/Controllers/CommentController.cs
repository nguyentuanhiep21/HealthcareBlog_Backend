using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// CommentController — chỉ xử lý HTTP request/response.
/// GlobalExceptionHandlerMiddleware bắt toàn bộ lỗi tập trung.
/// </summary>
[Route("api/comments")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IReportService _reportService;

    public CommentController(ICommentService commentService, IReportService reportService)
    {
        _commentService = commentService;
        _reportService = reportService;
    }

    [HttpGet("post/{postId:int}")]
    public async Task<ActionResult<List<ViewCommentDTO>>> GetComments(
        int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var comments = await _commentService.ViewCommentsAsync(userId, postId, page, pageSize);
        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDetailDTO>> CreateComment([FromBody] CreateCommentDTO dto)
    {
        var userId = User.GetUserId()!;
        var comment = await _commentService.CreateCommentAsync(userId, dto);
        return Ok(new { message = "Bình luận thành công.", data = comment, success = true });
    }

    [HttpPut("{commentId:int}")]
    [Authorize]
    public async Task<ActionResult<CommentDetailDTO>> UpdateComment(int commentId, [FromBody] UpdateCommentDTO dto)
    {
        var userId = User.GetUserId()!;
        var comment = await _commentService.UpdateCommentAsync(userId, commentId, dto);
        return Ok(new { message = "Cập nhật bình luận thành công.", data = comment, success = true });
    }

    [HttpDelete("{commentId:int}")]
    [Authorize]
    public async Task<ActionResult> DeleteComment(int commentId)
    {
        var userId = User.GetUserId()!;
        await _commentService.DeleteCommentAsync(userId, commentId);
        return Ok(new { message = "Xóa bình luận thành công.", success = true });
    }

    [HttpPost("{commentId:int}/like")]
    [Authorize]
    public async Task<ActionResult> LikeComment(int commentId)
    {
        var userId = User.GetUserId()!;
        await _commentService.LikeCommentAsync(userId, commentId);
        return Ok(new { message = "Thích bình luận thành công.", success = true });
    }

    [HttpDelete("{commentId:int}/like")]
    [Authorize]
    public async Task<ActionResult> UnlikeComment(int commentId)
    {
        var userId = User.GetUserId()!;
        await _commentService.UnlikeLikeCommentAsync(userId, commentId);
        return Ok(new { message = "Bỏ thích bình luận thành công.", success = true });
    }

    [HttpPost("{commentId:int}/report")]
    [Authorize]
    public async Task<ActionResult<ViewReportDTO>> ReportComment(int commentId, [FromBody] CreateCommentReportDTO dto)
    {
        var userId = User.GetUserId()!;
        var createReportDTO = new CreateReportDTO
        {
            ContentType = "Comment",
            ContentId = commentId.ToString(),
            Reason = dto.Reason,
            Description = dto.Description
        };

        var (report, isExisting) = await _reportService.CreateReportAsync(userId, createReportDTO);
        var message = isExisting ? "Bạn đã báo cáo bình luận này trước đó rồi." : "Báo cáo bình luận thành công.";

        return Ok(new { message, success = true, data = report });
    }

    [HttpDelete("admin/{commentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AdminDeleteComment(int commentId)
    {
        await _commentService.AdminDeleteCommentAsync(commentId);
        return Ok(new { message = "Xóa bình luận thành công.", success = true });
    }
}
