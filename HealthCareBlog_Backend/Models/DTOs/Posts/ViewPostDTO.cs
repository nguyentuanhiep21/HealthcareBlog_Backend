using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class ViewPostDTO
    {
        public int Id { get; set; } // ID bài viết
        public string AuthorId { get; set; } = string.Empty; // ID tác giả
        public DateTime UploadTime { get; set; } // Thời gian tải lên
        public string Content { get; set; } = string.Empty; // Nội dung bài viết
        public string? ImageUrl { get; set; } // URL ảnh đơn (nếu có)
        public int LikeCount { get; set; } // Số lượt thích
        public int CommentCount { get; set; } // Số bình luận
        public bool IsLikedByCurrentUser { get; set; } // Người dùng hiện tại đã thích bài viết chưa
        public bool IsSavedByCurrentUser { get; set; } // Người dùng hiện tại đã lưu bài viết chưa
        public DateTime CreatedAt { get; set; } // Thời gian tạo
        
        // Author info
        public AuthorDTO Author { get; set; } = new AuthorDTO();
    }
    
    public class AuthorDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsFollowing { get; set; }
    }
}
