namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class SuggestedUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int FollowerCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}
