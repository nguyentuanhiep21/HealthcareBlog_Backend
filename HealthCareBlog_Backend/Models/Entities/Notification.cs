using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("notifications")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Required]
    [Column("type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Column("sender_id")]
    public string? SenderId { get; set; }

    [Column("reference_id")]
    public int? ReferenceId { get; set; }

    [Column("reference_type")]
    [StringLength(50)]
    public string? ReferenceType { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
