using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class ReportMapper
    {
        public static ViewReportDTO ToViewReportDTO(
            this ReportedContent report, 
            User? reporter,
            User? targetUser = null,
            string? contentPreview = null)
        {
            // Use snapshot data if original user is deleted
            var finalReporterName = reporter?.FullName ?? reporter?.UserName ?? report.ReporterFullName ?? "Đã xóa";
            var finalReporterAvatar = reporter?.AvatarUrl; // Always use current avatar or null
            
            var finalTargetUserId = targetUser?.Id ?? report.TargetUserId;
            var finalTargetUserName = targetUser?.FullName ?? targetUser?.UserName ?? report.TargetUserFullName ?? "Đã xóa";
            var finalTargetUserAvatar = targetUser?.AvatarUrl; // Always use current avatar or null
            var finalTargetContent = contentPreview ?? targetUser?.Email ?? report.TargetContentSnapshot;

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
                ReporterId = report.ReporterId ?? string.Empty,
                ResolvedById = report.ResolvedById,
                
                // Reporter info - use snapshot if deleted
                ReportedByName = finalReporterName,
                ReportedByAvatar = finalReporterAvatar,
                
                // Target info - use snapshot if deleted
                TargetUserId = finalTargetUserId,
                TargetUserName = finalTargetUserName,
                TargetUserAvatar = finalTargetUserAvatar,
                TargetContent = finalTargetContent
            };
        }
    }
}
