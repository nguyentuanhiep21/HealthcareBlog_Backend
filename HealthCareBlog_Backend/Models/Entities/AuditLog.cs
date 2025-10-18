using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("audit_logs")]
public partial class AuditLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Required]
    [Column("admin_id")]
    public string AdminId { get; set; } = null!;

    [Required]
    [Column("action")]
    [StringLength(50)]
    public string Action { get; set; } = null!;

    [Required]
    [Column("target_type")]
    [StringLength(50)]
    public string TargetType { get; set; } = null!;

    [Required]
    [Column("target_id")]
    public int TargetId { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("AdminId")]
    public virtual ApplicationUser Admin { get; set; } = null!;
}
