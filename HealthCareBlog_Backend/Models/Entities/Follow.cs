using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("follows")]
public partial class Follow
{
    [Key]
    [Column("follow_id")]
    public int FollowId { get; set; }

    [Required]
    [Column("follower_id")]
    public string FollowerId { get; set; } = null!;

    [Required]
    [Column("following_id")]
    public string FollowingId { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("FollowerId")]
    public virtual ApplicationUser Follower { get; set; } = null!;

    [ForeignKey("FollowingId")]
    public virtual ApplicationUser FollowingUser { get; set; } = null!;
}
