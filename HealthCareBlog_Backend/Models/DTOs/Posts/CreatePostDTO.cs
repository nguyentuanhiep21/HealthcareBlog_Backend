namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class CreatePostDTO
    {
        public string Content { get; set; } = string.Empty; // Nội dung bài viết
        public List<string>? ImageUrls { get; set; } // Danh sách URL ảnh
    }
}
