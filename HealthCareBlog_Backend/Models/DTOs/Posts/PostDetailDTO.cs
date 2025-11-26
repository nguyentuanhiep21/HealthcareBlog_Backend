namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class PostDetailDTO
    {
        public int Id { get; set; } // Mã bài viết
        public string Content { get; set; } = string.Empty; // Nội dung bài viết
        public List<string>? ImageUrls { get; set; } // Danh sách URL ảnh
        public DateTime CreatedAt { get; set; } // Thời gian tạo bài viết
    }
}
