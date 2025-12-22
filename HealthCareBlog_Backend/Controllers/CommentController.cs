using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<List<ViewCommentDTO>>> GetComments(
            int postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            var comments = await _commentService.ViewCommentsAsync(userId, postId, page, pageSize);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDetailDTO>> CreateComment([FromBody] CreateCommentDTO createCommentDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var comment = await _commentService.CreateCommentAsync(userId, createCommentDTO);
            return CreatedAtAction(nameof(GetComments), new { postId = comment.PostId }, comment);
        }

        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<ActionResult<CommentDetailDTO>> UpdateComment(
            int commentId,
            [FromBody] UpdateCommentDTO updateCommentDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var comment = await _commentService.UpdateCommentAsync(userId, commentId, updateCommentDTO);
            return Ok(comment);
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _commentService.DeleteCommentAsync(userId, commentId);
            return Ok(new { message = "Comment deleted successfully." });
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> LikeComment(int commentId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _commentService.LikeCommentAsync(userId, commentId);
            return Ok(new { message = "Comment liked successfully." });
        }

        [HttpDelete("{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikeComment(int commentId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _commentService.UnlikeLikeCommentAsync(userId, commentId);
            return Ok(new { message = "Comment unliked successfully." });
        }

        [HttpPost("{commentId}/report")]
        [Authorize]
        public async Task<ActionResult<ViewReportDTO>> ReportComment(int commentId, [FromBody] CreateReportDTO createReportDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            createReportDTO.ContentType = "Comment";
            createReportDTO.ContentId = commentId.ToString();

            var report = await _reportService.CreateReportAsync(userId, createReportDTO);
            return Ok(new { message = "Comment reported successfully.", data = report });
        }
    }
}
