namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class UpdatePostDTO
    {
        public string Content { get; set; } = string.Empty; // Nội dung bài viết
        public string? ImageUrl { get; set; } // URL ảnh đơn (nếu có)
    }
}
