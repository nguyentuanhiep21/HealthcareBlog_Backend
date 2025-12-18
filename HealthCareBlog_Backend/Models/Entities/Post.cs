using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Posts - Lưu các bài viết của người dùng
/// Bài viết đơn giản, xóa là xóa thẳng trong DB, mỗi post chỉ có 1 ảnh
/// </summary>
[Table("posts")]
public class Post
{
  [Key]
  [Column("id")]
  public int Id { get; set; } // ID bài viết

  [Column("content")]
  [Required]
  public string Content { get; set; } = string.Empty; // Nội dung bài viết

  [Column("image_url")]
  [StringLength(500)]
  public string? ImageUrl { get; set; } // URL ảnh đơn (nếu có)

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } // Thời gian tạo bài

  // ========== COUNTERS ==========
  [Column("like_count")]
  public int LikeCount { get; set; } // Số lượt thích

  [Column("comment_count")]
  public int CommentCount { get; set; } // Số bình luận

  // ========== FOREIGN KEYS ==========
  [Column("user_id")]
  [StringLength(450)]
  [Required]
  public string UserId { get; set; } = string.Empty; // ID tác giả

  // ========== NAVIGATION PROPERTIES ==========
  [ForeignKey("UserId")]
  public virtual User User { get; set; } = null!; // Tác giả

  public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
  public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
