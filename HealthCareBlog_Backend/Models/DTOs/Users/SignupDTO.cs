using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class SignupDTO
    {
        public string? FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }
}
