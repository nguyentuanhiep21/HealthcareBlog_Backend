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
    }
}
