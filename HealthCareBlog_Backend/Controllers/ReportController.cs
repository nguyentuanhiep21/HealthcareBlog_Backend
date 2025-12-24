using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<ActionResult<ViewReportDTO>> CreateReport([FromBody] CreateReportDTO createReportDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var report = await _reportService.CreateReportAsync(userId, createReportDTO);
            return Ok(new { message = "Report created successfully.", data = report });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ViewReportDTO>>> GetAllReports(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? contentType = null)
        {
            var reports = await _reportService.GetAllReportsAsync(page, pageSize, status, contentType);
            return Ok(reports);
        }

        [HttpGet("{reportId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewReportDTO>> GetReportById(int reportId)
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            return Ok(report);
        }

        [HttpGet("my-reports")]
        public async Task<ActionResult<List<ViewReportDTO>>> GetMyReports(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var reports = await _reportService.GetUserReportsAsync(userId, page, pageSize);
            return Ok(reports);
        }

        [HttpPut("{reportId}/resolve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewReportDTO>> ResolveReport(int reportId, [FromBody] ResolveReportDTO resolveReportDTO)
        {
            var adminId = User.GetUserId();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("User not authenticated.");
            }

            var report = await _reportService.ResolveReportAsync(adminId, reportId, resolveReportDTO);
            return Ok(new { message = "Report resolved successfully.", data = report });
        }

        [HttpDelete("{reportId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteReport(int reportId)
        {
            var result = await _reportService.DeleteReportAsync(reportId);
            return Ok(new { message = "Report deleted successfully.", success = result });
        }

        [HttpPut("{reportId}/process-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewReportDTO>> ProcessUserReport(int reportId, [FromBody] ProcessUserReportDTO processReportDTO)
        {
            var adminId = User.GetUserId();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("User not authenticated.");
            }

            var report = await _reportService.ProcessUserReportAsync(adminId, reportId, processReportDTO);
            return Ok(new { message = "User report processed successfully.", data = report });
        }

        [HttpPut("{reportId}/process-post")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewReportDTO>> ProcessPostReport(int reportId, [FromBody] ProcessPostReportDTO processReportDTO)
        {
            var adminId = User.GetUserId();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("User not authenticated.");
            }

            var report = await _reportService.ProcessPostReportAsync(adminId, reportId, processReportDTO);
            return Ok(new { message = "Post report processed successfully.", data = report });
        }

        [HttpPut("{reportId}/process-comment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewReportDTO>> ProcessCommentReport(int reportId, [FromBody] ProcessCommentReportDTO processReportDTO)
        {
            var adminId = User.GetUserId();
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("User not authenticated.");
            }

            var report = await _reportService.ProcessCommentReportAsync(adminId, reportId, processReportDTO);
            return Ok(new { message = "Comment report processed successfully.", data = report });
        }
    }
}
