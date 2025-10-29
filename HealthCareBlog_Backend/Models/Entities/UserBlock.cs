using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng UserBlocks - Quản lý hành động chặn giữa người dùng
/// Lưu user nào chặn user nào, dùng để ngăn chặn tương tác và hiển thị thông tin chặn
/// </summary>
[Table("user_blocks")]
public class UserBlock
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của hành động chặn

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian chặn

    // ========== FOREIGN KEYS ==========
    [Column("blocker_id")]
    [StringLength(450)]
    [Required]
    public string BlockerId { get; set; } = string.Empty; // ID người chặn

    [Column("blocked_id")]
    [StringLength(450)]
    [Required]
    public string BlockedId { get; set; } = string.Empty; // ID người bị chặn

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("BlockerId")]
    public virtual ApplicationUser Blocker { get; set; } = null!; // Người chặn

    [ForeignKey("BlockedId")]
    public virtual ApplicationUser Blocked { get; set; } = null!; // Người bị chặn
}
