using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Notifications - Lưu các thông báo gửi tới người dùng
/// Bao gồm loại thông báo (like, comment, follow...), nội dung hiển thị, trạng thái đã đọc
/// Và tham chiếu đến đối tượng liên quan (post, comment)
/// </summary>
[Table("notifications")]
public class Notification
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID thông báo

    [Column("type")]
    public NotificationType Type { get; set; } // Loại thông báo (enum)

    [Column("content")]
    [Required]
    [StringLength(500)]
    public string Content { get; set; } = string.Empty; // Nội dung hiển thị cho thông báo

    [Column("is_read")]
    public bool IsRead { get; set; } // Đã được người dùng đọc chưa

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian tạo thông báo

    // ========== FOREIGN KEYS ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người nhận

    [Column("actor_id")]
    [StringLength(450)]
    public string? ActorId { get; set; } // ID người thực hiện hành động gây ra thông báo

    [Column("post_id")]
    public int? PostId { get; set; } // ID bài viết liên quan (nếu có)

    [Column("comment_id")]
    public int? CommentId { get; set; } // ID bình luận liên quan (nếu có)

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người nhận

    [ForeignKey("ActorId")]
    public virtual ApplicationUser? Actor { get; set; } // Người thực hiện (navigation)

    [ForeignKey("PostId")]
    public virtual Post? Post { get; set; } // Bài viết liên quan

    [ForeignKey("CommentId")]
    public virtual Comment? Comment { get; set; } // Bình luận liên quan
}
