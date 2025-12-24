using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Reports
{
    public class ProcessUserReportDTO
    {
        /// <summary>
        /// Action to take: "Lock", "Delete", or "Ignore"
        /// </summary>
        [Required(ErrorMessage = "Action is required")]
        public string Action { get; set; } = string.Empty;
    }
}
