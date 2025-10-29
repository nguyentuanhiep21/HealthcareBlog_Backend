using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng GroupPostPendings - Lưu bài viết gửi chờ duyệt trong nhóm
/// Lưu bài viết, trạng thái phê duyệt, thời gian gửi và người xử lý
/// Hỗ trợ chức năng duyệt bài của admin/moderator nhóm
/// </summary>
[Table("group_post_pendings")]
public class GroupPostPending
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID mục chờ duyệt

    [Column("submitted_at")]
    public DateTime SubmittedAt { get; set; } // Thời điểm gửi bài chờ duyệt

    [Column("is_approved")]
    public bool IsApproved { get; set; } // Trạng thái duyệt

    [Column("responsed_at")]
    public DateTime? ResponsedAt { get; set; } // Thời điểm admin phản hồi

    // ========== FOREIGN KEYS ==========
    [Column("group_id")]
    [Required]
    public int GroupId { get; set; } // ID nhóm

    [Column("post_id")]
    [Required]
    public int PostId { get; set; } // ID bài viết chờ

    [Column("responsed_by_id")]
    [StringLength(450)]
    public string? ResponsedById { get; set; } // ID admin xử lý

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!; // Nhóm

    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!; // Bài viết

    [ForeignKey("ResponsedById")]
    public virtual ApplicationUser? ResponsedBy { get; set; } // Người xử lý
}
