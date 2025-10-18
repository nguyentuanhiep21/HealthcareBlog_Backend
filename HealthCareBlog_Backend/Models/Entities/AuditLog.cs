using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng AuditLogs - Ghi lại lịch sử hành động của Admin
/// Lưu trữ mọi thao tác quản trị: xóa bài, ban user, chỉnh sửa...
/// Dùng để tracking và audit trail cho hệ thống
/// Đảm bảo tính minh bạch và có thể rollback khi cần
/// </summary>
[Table("audit_logs")]
public partial class AuditLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; } // ID log

    [Required]
    [Column("admin_id")]
    public string AdminId { get; set; } = null!; // ID admin thực hiện

    [Required]
    [Column("action")]
    [StringLength(50)]
    public string Action { get; set; } = null!; // Hành động: Delete, Ban, Update, Restore...

    [Required]
    [Column("target_type")]
    [StringLength(50)]
    public string TargetType { get; set; } = null!; // Loại đối tượng: User, Post, Comment...

    [Required]
    [Column("target_id")]
    public int TargetId { get; set; } // ID đối tượng bị tác động

    [Column("reason")]
    public string? Reason { get; set; } // Lý do thực hiện hành động

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian thực hiện

    // Navigation Properties
    [ForeignKey("AdminId")]
    public virtual ApplicationUser Admin { get; set; } = null!;
}
