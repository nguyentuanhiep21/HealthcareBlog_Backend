using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Comments - Quản lý bình luận của người dùng trên bài viết
/// Bình luận đơn giản, không có reply, xóa là xóa thẳng trong DB
/// </summary>
[Table("comments")]
public class Comment
{
  [Key]
  [Column("id")]
  public int Id { get; set; } // ID bình luận

  [Column("content")]
  [Required]
  [StringLength(2000)]
  public string Content { get; set; } = string.Empty; // Nội dung bình luận

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } // Thời gian tạo

  // ========== COUNTERS ==========
  [Column("like_count")]
  public int LikeCount { get; set; } // Số lượt thích

  // ========== FOREIGN KEYS ==========
  [Column("post_id")]
  [Required]
  public int PostId { get; set; } // ID bài viết liên kết

  [Column("user_id")]
  [StringLength(450)]
  [Required]
  public string UserId { get; set; } = string.Empty; // ID người tạo bình luận

  // ========== NAVIGATION PROPERTIES ==========
  [ForeignKey("PostId")]
  public virtual Post Post { get; set; } = null!; // Bài viết liên quan

  [ForeignKey("UserId")]
  public virtual User User { get; set; } = null!; // Tác giả bình luận

  public virtual ICollection<Like> Likes { get; set; } = new List<Like>(); // Lượt thích cho bình luận
}
