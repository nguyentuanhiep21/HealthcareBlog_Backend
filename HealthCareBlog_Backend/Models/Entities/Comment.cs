using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Comments - Quản lý bình luận trên bài viết
/// Hỗ trợ bình luận lồng nhau (reply comment) thông qua ParentCommentId
/// Lưu trữ nội dung bình luận và thời gian tạo/chỉnh sửa
/// Dùng để hiển thị chuỗi thảo luận dưới mỗi bài viết
/// </summary>
[Table("comments")]
public partial class Comment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; } // ID bình luận

    [Required]
    [Column("post_id")]
    public int PostId { get; set; } // ID bài đăng

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người bình luận

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!; // Nội dung bình luận

    [Column("parent_comment_id")]
    public int? ParentCommentId { get; set; } // ID bình luận cha (null nếu là comment gốc)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian bình luận

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian chỉnh sửa

    // Navigation Properties
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    [ForeignKey("ParentCommentId")]
    public virtual Comment? ParentComment { get; set; } // Bình luận cha

    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>(); // Các reply
}
