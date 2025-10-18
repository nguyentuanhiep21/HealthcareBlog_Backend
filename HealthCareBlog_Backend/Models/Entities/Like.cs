using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("likes")]
public partial class Like
{
    [Key]
    [Column("like_id")]
    public int LikeId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Required]
    [Column("post_id")]
    public int PostId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;
}
