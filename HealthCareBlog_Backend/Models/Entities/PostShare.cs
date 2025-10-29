using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng PostShares - Lưu thông tin chia sẻ bài viết
/// Ghi nhận ai chia sẻ bài viết nào, nội dung chia sẻ kèm theo và có thể chia sẻ vào nhóm
/// </summary>
[Table("post_shares")]
public class PostShare
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của hành động chia sẻ

    [Column("share_content")]
    [StringLength(1000)]
    public string? ShareContent { get; set; } // Nội dung thêm khi chia sẻ (optional)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian chia sẻ

    // ========== FOREIGN KEYS ==========
    [Column("post_id")]
    [Required]
    public int PostId { get; set; } // ID bài viết gốc

    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người chia sẻ

    [Column("group_id")]
    public int? GroupId { get; set; } // Nếu chia sẻ vào nhóm, id nhóm

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!; // Bài viết gốc

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người chia sẻ

    [ForeignKey("GroupId")]
    public virtual Group? Group { get; set; } // Nhóm nơi chia sẻ (nếu có)
}
