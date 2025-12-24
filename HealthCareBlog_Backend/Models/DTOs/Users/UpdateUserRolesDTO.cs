using System.ComponentModel.DataAnnotations;

namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    /// <summary>
    /// DTO for updating user roles (Admin use only via Swagger)
    /// </summary>
    public class UpdateUserRolesDTO
    {
        /// <summary>
        /// User ID to update roles for
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// List of roles to assign to the user. Valid values: "Admin", "User"
        /// </summary>
        [Required]
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for getting user roles
    /// </summary>
    public class UserRolesDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
