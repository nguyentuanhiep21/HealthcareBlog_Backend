using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng PostHashtags - Liên kết nhiều-nhiều giữa bài viết và hashtag
/// Chứa PostId và HashtagId để xác định hashtag xuất hiện trong bài đăng
/// </summary>
[Table("post_hashtags")]
public class PostHashtag
{
    [Column("post_id")]
    public int PostId { get; set; } // ID bài viết

    [Column("hashtag_id")]
    public int HashtagId { get; set; } // ID hashtag

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!; // Bài viết

    [ForeignKey("HashtagId")]
    public virtual Hashtag Hashtag { get; set; } = null!; // Hashtag
}
