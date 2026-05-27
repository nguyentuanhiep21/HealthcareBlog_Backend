using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

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
    public async Task<ActionResult<ViewReportDTO>> CreateReport([FromBody] CreateReportDTO dto)
    {
        var userId = User.GetUserId()!;
        var (report, isExisting) = await _reportService.CreateReportAsync(userId, dto);

        var message = isExisting
            ? "Bạn đã báo cáo nội dung này trước đó rồi."
            : "Report created successfully.";

        return Ok(new { message, data = report, isExisting });
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

    [HttpPut("{reportId:int}/process-user")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ViewReportDTO>> ProcessUserReport(int reportId, [FromBody] ProcessUserReportDTO dto)
    {
        var adminId = User.GetUserId()!;
        var report = await _reportService.ProcessUserReportAsync(adminId, reportId, dto);
        return Ok(new { message = "User report processed successfully.", data = report });
    }

    [HttpPut("{reportId:int}/process-post")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ViewReportDTO>> ProcessPostReport(int reportId, [FromBody] ProcessPostReportDTO dto)
    {
        var adminId = User.GetUserId()!;
        var report = await _reportService.ProcessPostReportAsync(adminId, reportId, dto);
        return Ok(new { message = "Post report processed successfully.", data = report });
    }

    [HttpPut("{reportId:int}/process-comment")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ViewReportDTO>> ProcessCommentReport(int reportId, [FromBody] ProcessCommentReportDTO dto)
    {
        var adminId = User.GetUserId()!;
        var report = await _reportService.ProcessCommentReportAsync(adminId, reportId, dto);
        return Ok(new { message = "Comment report processed successfully.", data = report });
    }
}
