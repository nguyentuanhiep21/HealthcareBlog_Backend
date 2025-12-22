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
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var comment = await _commentService.CreateCommentAsync(userId, createCommentDTO);
                return Ok(new { message = "Bình luận thành công.", data = comment, success = true });
            }
            catch (HealthCareBlog_Backend.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (HealthCareBlog_Backend.Exceptions.BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentController] CreateComment Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<ActionResult<CommentDetailDTO>> UpdateComment(
            int commentId,
            [FromBody] UpdateCommentDTO updateCommentDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var comment = await _commentService.UpdateCommentAsync(userId, commentId, updateCommentDTO);
                return Ok(new { message = "Cập nhật bình luận thành công.", data = comment, success = true });
            }
            catch (HealthCareBlog_Backend.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (HealthCareBlog_Backend.Exceptions.UnauthorizedException ex)
            {
                return StatusCode(403, new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentController] UpdateComment Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _commentService.DeleteCommentAsync(userId, commentId);
                return Ok(new { message = "Xóa bình luận thành công.", success = true });
            }
            catch (HealthCareBlog_Backend.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (HealthCareBlog_Backend.Exceptions.UnauthorizedException ex)
            {
                return StatusCode(403, new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentController] DeleteComment Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> LikeComment(int commentId)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _commentService.LikeCommentAsync(userId, commentId);
                return Ok(new { message = "Thích bình luận thành công.", success = true });
            }
            catch (HealthCareBlog_Backend.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (HealthCareBlog_Backend.Exceptions.BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentController] LikeComment Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete("{commentId}/like")]
        [Authorize]
        public async Task<ActionResult> UnlikeComment(int commentId)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Người dùng chưa đăng nhập.", success = false });
                }

                var result = await _commentService.UnlikeLikeCommentAsync(userId, commentId);
                return Ok(new { message = "Bỏ thích bình luận thành công.", success = true });
            }
            catch (HealthCareBlog_Backend.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (HealthCareBlog_Backend.Exceptions.BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentController] UnlikeComment Exception: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
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
