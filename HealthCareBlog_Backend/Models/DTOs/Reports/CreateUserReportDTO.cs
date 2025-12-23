using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Reports
{
    public class CreateUserReportDTO
    {
        [Required(ErrorMessage = "Lý do là bắt buộc")]
        [StringLength(100, ErrorMessage = "Lý do không được vượt quá 100 ký tự")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Chi tiết không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }
    }
}
