using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng AuditLogs - Ghi nhận các hành động quản trị và sự kiện hệ thống
/// Lưu loại hành động, thực thể bị tác động, giá trị cũ/mới, lý do, admin thực hiện và IP
/// Dùng để truy vết thay đổi, kiểm tra bảo mật và lưu lịch sử quản trị
/// </summary>
[Table("audit_logs")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID bản ghi audit

    [Column("action_type")]
    [Required]
    [StringLength(50)]
    public string ActionType { get; set; } = string.Empty; // Loại hành động (DeletePost, BanUser...)

    [Column("entity_type")]
    [Required]
    [StringLength(50)]
    public string EntityType { get; set; } = string.Empty; // Loại thực thể bị tác động (Post, Comment, User...)

    [Column("entity_id")]
    [Required]
    [StringLength(450)]
    public string EntityId { get; set; } = string.Empty; // ID của thực thể bị tác động

    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; } // Lý do ngắn gọn cho hành động

    [Column("description")]
    [StringLength(2000)]
    public string? Description { get; set; } // Mô tả chi tiết của hành động

    [Column("old_value")]
    public string? OldValue { get; set; } // Giá trị cũ (nếu cần)

    [Column("new_value")]
    public string? NewValue { get; set; } // Giá trị mới (nếu cần)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian ghi nhận

    // ========== FOREIGN KEYS ==========
    [Column("admin_id")]
    [StringLength(450)]
    [Required]
    public string AdminId { get; set; } = string.Empty; // ID admin thực hiện hành động

    [Column("target_user_id")]
    [StringLength(450)]
    public string? TargetUserId { get; set; } // ID người dùng (nếu hành động liên quan đến user)

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("AdminId")]
    public virtual User Admin { get; set; } = null!; // Admin thực hiện

    [ForeignKey("TargetUserId")]
    public virtual User? TargetUser { get; set; } // Người dùng liên quan
}
