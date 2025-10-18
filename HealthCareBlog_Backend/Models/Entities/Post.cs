using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Posts - Quản lý bài đăng của người dùng
/// Lưu trữ nội dung bài viết, ảnh/video đính kèm
/// Thống kê số lượt thích, bình luận, lượt xem
/// Hỗ trợ tính năng tìm kiếm bài viết bằng AI
/// </summary>
[Table("posts")]
public partial class Post
{
    [Key]
    [Column("post_id")]
    public int PostId { get; set; } // ID bài đăng

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID tác giả

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!; // Nội dung bài viết

    [Column("media_urls")]
    public string? MediaUrls { get; set; } // JSON array chứa URL ảnh/video

    [Column("likes_count")]
    public int LikesCount { get; set; } = 0; // Số lượt thích (denormalized để query nhanh)

    [Column("comments_count")]
    public int CommentsCount { get; set; } = 0; // Số lượt bình luận

    [Column("views_count")]
    public int ViewsCount { get; set; } = 0; // Số lượt xem

    [Column("is_published")]
    public bool IsPublished { get; set; } = true; // Trạng thái công khai/nháp

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian đăng bài

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian chỉnh sửa cuối

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
