using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("reported_contents")]
public partial class ReportedContent
{
    [Key]
    [Column("report_id")]
    public int ReportId { get; set; }

    [Required]
    [Column("reporter_id")]
    public string ReporterId { get; set; } = null!;

    [Required]
    [Column("content_type")]
    [StringLength(50)]
    public string ContentType { get; set; } = null!;

    [Required]
    [Column("content_id")]
    public int ContentId { get; set; }

    [Required]
    [Column("reason")]
    public string Reason { get; set; } = null!;

    [Required]
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    [Column("reviewed_by")]
    public string? ReviewedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }

    // Navigation Properties
    [ForeignKey("ReporterId")]
    public virtual ApplicationUser Reporter { get; set; } = null!;
}
