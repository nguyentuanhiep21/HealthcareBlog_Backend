using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng GroupJoinRequests - Lưu yêu cầu tham gia nhóm
/// Lưu user yêu cầu, trạng thái duyệt, thời gian và người xử lý
/// Dùng cho luồng quản lý thành viên nhóm (approve/deny)
/// </summary>
[Table("group_join_requests")]
public class GroupJoinRequest
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID yêu cầu

    [Column("requested_at")]
    public DateTime RequestedAt { get; set; } // Thời điểm gửi yêu cầu

    [Column("is_approved")]
    public bool IsApproved { get; set; } // Trạng thái duyệt

    [Column("responsed_at")]
    public DateTime? ResponsedAt { get; set; } // Thời gian admin phản hồi

    // ========== FOREIGN KEYS ==========
    [Column("group_id")]
    [Required]
    public int GroupId { get; set; } // ID nhóm

    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người yêu cầu

    [Column("responsed_by_id")]
    [StringLength(450)]
    public string? ResponsedById { get; set; } // ID admin/manager phản hồi

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!; // Nhóm liên quan

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người gửi yêu cầu

    [ForeignKey("ResponsedById")]
    public virtual ApplicationUser? ResponsedBy { get; set; } // Người xử lý yêu cầu
}
