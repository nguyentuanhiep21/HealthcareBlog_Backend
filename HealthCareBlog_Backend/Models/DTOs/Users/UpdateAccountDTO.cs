using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class UpdateAccountDTO
    {
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
        public string? PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        [StringLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters")]
        public string? AvatarUrl { get; set; }
    }
}
