using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("comments")]
public partial class Comment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Required]
    [Column("post_id")]
    public int PostId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("parent_comment_id")]
    public int? ParentCommentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    [ForeignKey("ParentCommentId")]
    public virtual Comment? ParentComment { get; set; }

    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
