namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class PostDetailDTO
    {
        public int Id { get; set; } // Mã bài viết
        public string Content { get; set; } = string.Empty; // Nội dung bài viết
        public string? ImageUrl { get; set; } // URL ảnh đơn (nếu có)
        public DateTime CreatedAt { get; set; } // Thời gian tạo bài viết
        public int LikeCount { get; set; } // Số lượt thích
        public int CommentCount { get; set; } // Số bình luận
        public string AuthorId { get; set; } = string.Empty; // ID tác giả
    }
}
