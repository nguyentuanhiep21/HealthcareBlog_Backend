using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Likes - Lưu trữ lượt thích của người dùng cho bài viết hoặc bình luận
/// Dùng để tính số lượng like, hiển thị ai đã like và kích hoạt thông báo
/// </summary>
[Table("likes")]
public class Like
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của lượt thích

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian người dùng like

    // ========== FOREIGN KEYS ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người dùng đã like

    [Column("post_id")]
    public int? PostId { get; set; } // Nếu like bài viết, id bài viết

    [Column("comment_id")]
    public int? CommentId { get; set; } // Nếu like bình luận, id bình luận

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!; // Người đã like

    [ForeignKey("PostId")]
    public virtual Post? Post { get; set; } // Bài viết được like (nếu có)

    [ForeignKey("CommentId")]
    public virtual Comment? Comment { get; set; } // Bình luận được like (nếu có)
}
