using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Hashtags - Lưu hashtag được sử dụng trong hệ thống
/// Lưu tên, số lần sử dụng và timestamps; liên kết tới các bài viết qua PostHashtag
/// Dùng để tìm kiếm, thống kê và gợi ý hashtag cho nội dung
/// </summary>
[Table("hashtags")]
public class Hashtag
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID hashtag

    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty; // Tên hashtag

    [Column("usage_count")]
    public int UsageCount { get; set; } // Số lần hashtag được sử dụng

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời điểm tạo hashtag

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật

    // ========== NAVIGATION PROPERTIES ==========
    public virtual ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>(); // Liên kết bài viết
}
