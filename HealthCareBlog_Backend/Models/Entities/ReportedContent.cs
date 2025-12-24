using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("reported_contents")]
public class ReportedContent
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("content_type")]
    [Required]
    [StringLength(50)]
    public string ContentType { get; set; } = string.Empty;

    [Column("content_id")]
    [Required]
    [StringLength(450)]
    public string ContentId { get; set; } = string.Empty;

    [Column("reason")]
    [Required]
    [StringLength(100)]
    public string Reason { get; set; } = string.Empty;

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Column("status")]
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }

    [Column("admin_note")]
    [StringLength(1000)]
    public string? AdminNote { get; set; }

    [Column("reporter_id")]
    [StringLength(450)]
    public string? ReporterId { get; set; }

    [Column("resolved_by_id")]
    [StringLength(450)]
    public string? ResolvedById { get; set; }

    // Snapshot fields - lưu thông tin tại thời điểm báo cáo
    [Column("reporter_fullname")]
    [StringLength(255)]
    public string? ReporterFullName { get; set; }

    [Column("target_user_id")]
    [StringLength(450)]
    public string? TargetUserId { get; set; }

    [Column("target_user_fullname")]
    [StringLength(255)]
    public string? TargetUserFullName { get; set; }

    [Column("target_content_snapshot")]
    [StringLength(2000)]
    public string? TargetContentSnapshot { get; set; }

    [ForeignKey("ReporterId")]
    public virtual User? Reporter { get; set; }

    [ForeignKey("ResolvedById")]
    public virtual User? ResolvedBy { get; set; }
}
