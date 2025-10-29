using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Groups - Quản lý nhóm người dùng
/// Lưu thông tin nhóm (tên, mô tả, ảnh), cài đặt riêng tư, counters và trạng thái
/// Dùng để tạo không gian cộng đồng, quản lý thành viên và bài viết trong nhóm
/// </summary>
[Table("groups")]
public class Group
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID nhóm

    [Column("name")]
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty; // Tên nhóm

    [Column("description")]
    [StringLength(2000)]
    public string? Description { get; set; } // Mô tả nhóm

    [Column("avatar_url")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; } // Ảnh đại diện nhóm

    [Column("cover_image_url")]
    [StringLength(500)]
    public string? CoverImageUrl { get; set; } // Ảnh bìa nhóm

    // ========== GROUP SETTINGS ==========
    [Column("privacy_type")]
    public GroupPrivacyType PrivacyType { get; set; } // Quyền riêng tư (Công khai / Riêng tư)

    [Column("require_post_approval")]
    public bool RequirePostApproval { get; set; } // Cần duyệt bài khi thành viên đăng

    [Column("require_member_approval")]
    public bool RequireMemberApproval { get; set; } // Cần duyệt member khi xin tham gia

    // ========== COUNTERS ==========
    [Column("member_count")]
    public int MemberCount { get; set; } // Số lượng thành viên (cache)

    [Column("post_count")]
    public int PostCount { get; set; } // Số bài viết trong nhóm (cache)

    // ========== SYSTEM INFO ==========
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian tạo nhóm

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật cuối

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } // Cờ xóa mềm nhóm

    [Column("status")]
    public GroupStatus Status { get; set; } = GroupStatus.Active; // Trạng thái hoạt động

    // ========== DEACTIVATION INFO ==========
    [Column("deactivated_at")]
    public DateTime? DeactivatedAt { get; set; } // Thời gian vô hiệu hóa

    [Column("deactivated_by")]
    [StringLength(450)]
    public string? DeactivatedBy { get; set; } // Admin vô hiệu hóa

    [Column("deactivation_reason")]
    [StringLength(1000)]
    public string? DeactivationReason { get; set; } // Lý do vô hiệu hóa

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; } // Thời gian xóa

    [Column("deleted_by")]
    [StringLength(450)]
    public string? DeletedBy { get; set; } // ID người xóa

    // ========== FOREIGN KEYS ==========
    [Column("owner_id")]
    [StringLength(450)]
    [Required]
    public string OwnerId { get; set; } = string.Empty; // Chủ nhóm (user id)

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("OwnerId")]
    public virtual ApplicationUser Owner { get; set; } = null!; // Owner navigation

    [ForeignKey("DeactivatedBy")]
    public virtual ApplicationUser? DeactivatedByAdmin { get; set; } // Admin đã vô hiệu hóa

    [ForeignKey("DeletedBy")]
    public virtual ApplicationUser? DeletedByUser { get; set; } // Người xóa nhóm

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>(); // Thành viên
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>(); // Bài viết trong nhóm
    public virtual ICollection<GroupJoinRequest> JoinRequests { get; set; } = new List<GroupJoinRequest>(); // Yêu cầu tham gia
    public virtual ICollection<GroupPostPending> PendingPosts { get; set; } = new List<GroupPostPending>(); // Bài chờ duyệt
}
