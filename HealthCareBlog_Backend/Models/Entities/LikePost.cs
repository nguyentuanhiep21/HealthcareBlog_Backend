using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// B?ng LikePost - L?u tr? l??t thích c?a ng??i dùng cho bài vi?t
/// Dùng ?? tính s? l??ng like, hi?n th? ai ?ã like và kích ho?t thông báo
/// </summary>
[Table("like_posts")]
public class LikePost
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID c?a l??t thích

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Th?i gian ng??i dùng like

    // ========== FOREIGN KEYS ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID ng??i dùng ?ã like

    [Column("post_id")]
    [Required]
    public int PostId { get; set; } // ID bài vi?t ???c like

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!; // Ng??i ?ã like

    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!; // Bài vi?t ???c like
}
