using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng ReportedContents - Quản lý báo cáo vi phạm từ người dùng
/// User có thể báo cáo post, comment, hoặc user khác vi phạm
/// Lưu trữ lý do báo cáo, trạng thái xử lý (Pending/Reviewed/Resolved)
/// Admin sẽ xem xét và xử lý các báo cáo này
/// </summary>
[Table("reported_contents")]
public partial class ReportedContent
{
    [Key]
    [Column("report_id")]
    public int ReportId { get; set; } // ID báo cáo

    [Required]
    [Column("reporter_id")]
    public string ReporterId { get; set; } = null!; // ID người báo cáo

    [Required]
    [Column("content_type")]
    [StringLength(50)]
    public string ContentType { get; set; } = null!; // Loại: Post, Comment, User

    [Required]
    [Column("content_id")]
    public int ContentId { get; set; } // ID nội dung bị báo cáo

    [Required]
    [Column("reason")]
    public string Reason { get; set; } = null!; // Lý do báo cáo: spam, harassment, inappropriate...

    [Required]
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Trạng thái: Pending, Reviewed, Resolved

    [Column("reviewed_by")]
    public string? ReviewedBy { get; set; } // ID admin xử lý

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian báo cáo

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; } // Thời gian xử lý xong

    // Navigation Properties
    [ForeignKey("ReporterId")]
    public virtual ApplicationUser Reporter { get; set; } = null!;
}
