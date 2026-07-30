using HealthCareBlog_Backend.Models.DTOs.Comments;

namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class PostDetailDTO
    {
        public int Id { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public AuthorDTO Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; } // backward compat
        public List<string> ImageUrls { get; set; } = new List<string>(); // Danh sách URL ảnh (tối đa 5)
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
    }
}
