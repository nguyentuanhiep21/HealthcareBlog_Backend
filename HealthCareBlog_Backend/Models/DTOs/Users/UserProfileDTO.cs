using HealthCareBlog_Backend.Models.DTOs.Posts;

namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class UserProfileDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? BannerUrl { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
        public List<ViewPostDTO> Posts { get; set; } = new List<ViewPostDTO>();
    }
}
