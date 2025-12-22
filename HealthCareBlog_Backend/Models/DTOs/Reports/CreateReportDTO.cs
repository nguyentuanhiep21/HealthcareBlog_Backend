using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Reports
{
    public class CreateReportDTO
    {
        [Required(ErrorMessage = "Content type is required")]
        public string ContentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content ID is required")]
        public string ContentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(100, ErrorMessage = "Reason cannot exceed 100 characters")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }
}
