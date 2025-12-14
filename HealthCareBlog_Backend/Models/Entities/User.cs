using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Users - Kế thừa IdentityUser để tạo liên kết với các bảng khác trong hệ thống
/// Class này chỉ có mục đích làm cầu nối giữa IdentityUser và các entities khác
/// Tất cả thông tin người dùng được quản lý bởi IdentityUser
/// </summary>
[Table("users")]
public class User : IdentityUser
{
    // ========== NAVIGATION PROPERTIES ==========
    // Bài viết, bình luận, lượt thích
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    // Follow
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

    // Block
    public virtual ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
    public virtual ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>();

    // Report
    public virtual ICollection<ReportedContent> Reports { get; set; } = new List<ReportedContent>();

    // Notifications
    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
}
