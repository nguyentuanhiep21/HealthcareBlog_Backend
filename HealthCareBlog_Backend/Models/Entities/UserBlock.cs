using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("saved_posts")]
public class SavedPost
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của hành động lưu bài viết

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian lưu bài viết

    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người dùng lưu bài viết

    [Column("post_id")]
    [Required]
    public int PostId { get; set; } // ID bài viết được lưu

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!; // Người dùng đã lưu bài viết

    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!; // Bài viết được lưu
}
