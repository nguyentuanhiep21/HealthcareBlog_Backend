using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Reports
{
    public class ResolveReportDTO
    {
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Admin note cannot exceed 1000 characters")]
        public string? AdminNote { get; set; }
    }
}
