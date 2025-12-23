namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class FollowUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
}