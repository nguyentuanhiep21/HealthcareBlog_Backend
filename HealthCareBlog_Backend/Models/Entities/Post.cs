using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("posts")]
public partial class Post
{
    [Key]
    [Column("post_id")]
    public int PostId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("media_urls")]
    public string? MediaUrls { get; set; }

    [Column("likes_count")]
    public int LikesCount { get; set; } = 0;

    [Column("comments_count")]
    public int CommentsCount { get; set; } = 0;

    [Column("views_count")]
    public int ViewsCount { get; set; } = 0;

    [Column("is_published")]
    public bool IsPublished { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
