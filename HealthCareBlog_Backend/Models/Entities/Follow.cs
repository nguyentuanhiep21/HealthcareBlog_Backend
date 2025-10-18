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
public partial class Follow
{
    [Key]
    [Column("follow_id")]
    public int FollowId { get; set; } // ID của mối quan hệ follow

    [Required]
    [Column("follower_id")]
    public string FollowerId { get; set; } = null!; // ID người theo dõi

    [Required]
    [Column("following_id")]
    public string FollowingId { get; set; } = null!; // ID người được theo dõi

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian bắt đầu follow

    // Navigation Properties
    [ForeignKey("FollowerId")]
    public virtual ApplicationUser Follower { get; set; } = null!;

    [ForeignKey("FollowingId")]
    public virtual ApplicationUser FollowingUser { get; set; } = null!;
}
