namespace HealthCareBlog_Backend.Models.DTOs.Reports
{
    public class ViewReportDTO
    {
        public int Id { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string ContentId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? AdminNote { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public string? ResolvedById { get; set; }

        // Reporter Info
        public string ReportedByName { get; set; } = string.Empty;
        public string? ReportedByAvatar { get; set; }

        // Target Info (User/Post Author/Comment Author)
        public string? TargetUserId { get; set; }
        public string? TargetUserName { get; set; }
        public string? TargetUserAvatar { get; set; }
        
        // Content preview (Post content or Comment content)
        public string? TargetContent { get; set; }
    }
}
