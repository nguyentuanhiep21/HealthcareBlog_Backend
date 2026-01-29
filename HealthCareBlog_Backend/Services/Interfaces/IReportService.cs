using HealthCareBlog_Backend.Models.DTOs.Reports;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IReportService
    {
        Task<(ViewReportDTO report, bool isExisting)> CreateReportAsync(string reporterId, CreateReportDTO createReportDTO);
        Task<List<ViewReportDTO>> GetAllReportsAsync(int page = 1, int pageSize = 20, string? status = null, string? contentType = null);
        Task<ViewReportDTO> ProcessUserReportAsync(string adminId, int reportId, ProcessUserReportDTO processReportDTO);
        Task<ViewReportDTO> ProcessPostReportAsync(string adminId, int reportId, ProcessPostReportDTO processReportDTO);
        Task<ViewReportDTO> ProcessCommentReportAsync(string adminId, int reportId, ProcessCommentReportDTO processReportDTO);
    }
}
