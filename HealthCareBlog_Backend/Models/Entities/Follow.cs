using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Follows - Quản lý mối quan hệ theo dõi giữa người dùng
/// Lưu trữ thông tin ai đang follow ai (many-to-many relationship)
/// Dùng để hiển thị danh sách người theo dõi, người đang theo dõi
/// Và để lọc bài viết từ những người mà user đang follow
/// </summary>
[Table("follows")]
public class Follow
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của mối quan hệ follow

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian bắt đầu follow

    // ========== FOREIGN KEYS ==========
    [Column("follower_id")]
    [StringLength(450)]
    [Required]
    public string FollowerId { get; set; } = string.Empty; // ID người theo dõi

    [Column("following_id")]
    [StringLength(450)]
    [Required]
    public string FollowingId { get; set; } = string.Empty; // ID người được theo dõi

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("FollowerId")]
    public virtual ApplicationUser Follower { get; set; } = null!; // Người theo dõi (navigation)

    [ForeignKey("FollowingId")]
    public virtual ApplicationUser FollowingUser { get; set; } = null!; // Người được theo dõi (navigation)
}
