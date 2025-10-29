using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng GroupRoles - Danh sách vai trò trong nhóm (Owner, Admin, Member)
/// Lưu tên và mô tả vai trò, dùng để phân quyền và gán quyền trong nhóm
/// </summary>
[Table("group_roles")]
public class GroupRole
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID vai trò trong nhóm

    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty; // Tên vai trò (Owner, Admin, Member)

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; } // Mô tả quyền hạn

    // ========== NAVIGATION PROPERTIES ==========
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>(); // Người dùng có vai trò này
}
