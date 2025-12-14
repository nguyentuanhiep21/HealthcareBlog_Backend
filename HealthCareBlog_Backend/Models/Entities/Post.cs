using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Posts - Lưu các bài viết của người dùng
/// Chứa nội dung, hình ảnh (URL list), counters và metadata phục vụ feed và chức năng quản trị
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

    [Column("image_urls")]
    [StringLength(2000)]
    public string? ImageUrls { get; set; } // Danh sách URL ảnh (có thể lưu dưới dạng JSON)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian tạo bài

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật cuối

    [Column("is_edited")]
    public bool IsEdited { get; set; } // Đã chỉnh sửa hay chưa

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } // Cờ xóa mềm

    [Column("view_count")]
    public int ViewCount { get; set; } // Lượt xem (cache)

    // ========== COUNTERS ==========
    [Column("like_count")]
    public int LikeCount { get; set; } // Số lượt thích

    [Column("comment_count")]
    public int CommentCount { get; set; } // Số bình luận

    // ========== DELETION INFO ==========
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; } // Thời gian xóa

    [Column("deleted_by")]
    [StringLength(450)]
    public string? DeletedBy { get; set; } // ID người xóa

    [Column("deletion_reason")]
    [StringLength(1000)]
    public string? DeletionReason { get; set; } // Lý do xóa

    [Column("is_deleted_by_admin")]
    public bool IsDeletedByAdmin { get; set; } // Xóa bởi admin hay user

    // ========== FOREIGN KEYS ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID tác giả

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!; // Tác giả

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; } // Người xóa (navigation)

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
