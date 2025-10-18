using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Likes - Quản lý lượt thích bài viết
/// Lưu trữ thông tin ai đã thích bài viết nào
/// Đảm bảo mỗi user chỉ thích 1 lần cho mỗi bài (unique constraint)
/// Dùng để hiển thị danh sách người thích và kiểm tra user đã thích chưa
/// </summary>
[Table("likes")]
public partial class Like
{
    [Key]
    [Column("like_id")]
    public int LikeId { get; set; } // ID lượt thích

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người thích

    [Required]
    [Column("post_id")]
    public int PostId { get; set; } // ID bài đăng được thích

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian thích

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;
}
