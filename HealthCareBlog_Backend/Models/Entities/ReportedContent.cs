using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng ReportedContents - Quản lý các báo cáo vi phạm nội dung
/// Lưu loại nội dung (Post, Comment, User...), id nội dung, lý do và trạng thái xử lý
/// Dùng cho luồng moderation và lịch sử xử lý báo cáo
/// </summary>
[Table("reported_contents")]
public class ReportedContent
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID báo cáo

    [Column("content_type")]
    [Required]
    [StringLength(50)]
    public string ContentType { get; set; } = string.Empty; // Loại nội dung: "Post", "Comment", "User"

    [Column("content_id")]
    [Required]
    [StringLength(450)]
    public string ContentId { get; set; } = string.Empty; // ID của nội dung được báo cáo

    [Column("reason")]
    [Required]
    [StringLength(100)]
    public string Reason { get; set; } = string.Empty; // Lý do báo cáo (tên ngắn)

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; } // Mô tả chi tiết từ người báo cáo

    [Column("status")]
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Trạng thái: "Pending", "Resolved", "Rejected"

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời điểm báo cáo

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; } // Thời điểm xử lý xong

    [Column("admin_note")]
    [StringLength(1000)]
    public string? AdminNote { get; set; } // Ghi chú của admin khi xử lý

    // ========== FOREIGN KEYS ==========
    [Column("reporter_id")]
    [StringLength(450)]
    [Required]
    public string ReporterId { get; set; } = string.Empty; // ID người báo cáo

    [Column("resolved_by_id")]
    [StringLength(450)]
    public string? ResolvedById { get; set; } // ID admin xử lý (nếu có)

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("ReporterId")]
    public virtual ApplicationUser Reporter { get; set; } = null!; // Người báo cáo

    [ForeignKey("ResolvedById")]
    public virtual ApplicationUser? ResolvedBy { get; set; } // Admin đã xử lý
}
