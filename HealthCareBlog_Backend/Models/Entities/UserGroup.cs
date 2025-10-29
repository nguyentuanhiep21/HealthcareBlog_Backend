using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng UserGroups - Liên kết người dùng với nhóm và vai trò của họ
/// Lưu thời gian gia nhập, vai trò và tham chiếu đến user/group/role
/// Dùng để quản lý thành viên, quyền hạn và thống kê nhóm
/// </summary>
[Table("user_groups")]
public class UserGroup
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID record thành viên

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } // Thời gian tham gia nhóm

    // ========== FOREIGN KEYS ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người dùng

    [Column("group_id")]
    [Required]
    public int GroupId { get; set; } // ID nhóm

    [Column("role_id")]
    [Required]
    public int RoleId { get; set; } // ID vai trò trong nhóm

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người dùng

    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!; // Nhóm

    [ForeignKey("RoleId")]
    public virtual GroupRole Role { get; set; } = null!; // Vai trò
}
