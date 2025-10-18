using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Notifications - Quản lý thông báo cho người dùng
/// Gửi thông báo khi có follow mới, like, comment, message
/// Lưu trữ loại thông báo, người gửi, đối tượng liên quan
/// Theo dõi trạng thái đã đọc để hiển thị badge số thông báo mới
/// </summary>
[Table("notifications")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; } // ID thông báo

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người nhận thông báo

    [Required]
    [Column("type")]
    [StringLength(50)]
    public string Type { get; set; } = null!; // Loại: Follow, Like, Comment, Message

    [Column("sender_id")]
    public string? SenderId { get; set; } // ID người gửi (người thực hiện hành động)

    [Column("reference_id")]
    public int? ReferenceId { get; set; } // ID đối tượng liên quan (PostId, CommentId...)

    [Column("reference_type")]
    [StringLength(50)]
    public string? ReferenceType { get; set; } // Loại đối tượng: Post, Comment, User...

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!; // Nội dung thông báo hiển thị

    [Column("is_read")]
    public bool IsRead { get; set; } = false; // Trạng thái đã đọc

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo thông báo

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
