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

            var (report, isExisting) = await _reportService.CreateReportAsync(userId, createReportDTO);
            
            if (isExisting)
            {
                return Ok(new { 
                    message = "Bạn đã báo cáo nội dung này trước đó rồi.", 
                    data = report,
                    isExisting = true
                });
            }

            return Ok(new { 
                message = "Report created successfully.", 
                data = report,
                isExisting = false
            });
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
