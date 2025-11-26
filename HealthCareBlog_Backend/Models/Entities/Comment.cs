using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Comments - Quản lý bình luận của người dùng trên bài viết
/// Hỗ trợ reply 1 cấp (ParentCommentId), lưu like bình luận và metadata xóa/duyệt
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

  [Column("updated_at")]
  public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật cuối (nếu có)

  [Column("is_edited")]
  public bool IsEdited { get; set; } // Đã chỉnh sửa hay chưa

  [Column("is_deleted")]
  public bool IsDeleted { get; set; } // Cờ xóa mềm

  // ========== COUNTERS ==========
  [Column("like_count")]
  public int LikeCount { get; set; } // Số lượt thích

  // ========== DELETION INFO ==========
  [Column("deleted_at")]
  public DateTime? DeletedAt { get; set; } // Thời gian xóa (nếu có)

  [Column("deleted_by")]
  [StringLength(450)]
  public string? DeletedBy { get; set; } // ID người xóa (admin hoặc tác giả)

  [Column("deletion_reason")]
  [StringLength(1000)]
  public string? DeletionReason { get; set; } // Lý do xóa (nếu có)

  [Column("is_deleted_by_admin")]
  public bool IsDeletedByAdmin { get; set; } // Xóa bởi admin hay tự xóa

  // ========== FOREIGN KEYS ==========
  [Column("post_id")]
  [Required]
  public int PostId { get; set; } // ID bài viết liên kết

  [Column("user_id")]
  [StringLength(450)]
  [Required]
  public string UserId { get; set; } = string.Empty; // ID người tạo bình luận

  [Column("parent_comment_id")]
  public int? ParentCommentId { get; set; } // ID bình luận cha nếu là reply (chỉ 1 cấp)

  // ========== NAVIGATION PROPERTIES ==========
  [ForeignKey("PostId")]
  public virtual Post Post { get; set; } = null!; // Bài viết liên quan

  [ForeignKey("UserId")]
  public virtual ApplicationUser User { get; set; } = null!; // Tác giả bình luận

  [ForeignKey("ParentCommentId")]
  public virtual Comment? ParentComment { get; set; } // Bình luận cha (nếu có)

  [ForeignKey("DeletedBy")]
  public virtual ApplicationUser? DeletedByUser { get; set; } // Người đã xóa (navigation)

  public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>(); // Danh sách reply con
  public virtual ICollection<Like> Likes { get; set; } = new List<Like>(); // Lượt thích cho bình luận
}
