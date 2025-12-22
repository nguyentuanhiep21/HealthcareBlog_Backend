using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class ReportMapper
    {
        public static ViewReportDTO ToViewReportDTO(this ReportedContent report)
        {
            return new ViewReportDTO
            {
                Id = report.Id,
                ContentType = report.ContentType,
                ContentId = report.ContentId,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                ResolvedAt = report.ResolvedAt,
                AdminNote = report.AdminNote,
                ReporterId = report.ReporterId,
                ResolvedById = report.ResolvedById
            };
        }
    }
}
