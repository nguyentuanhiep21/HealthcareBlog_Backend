using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
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
    // ========== OVERRIDE IDENTITYUSER PROPERTIES TO CONTROL COLUMN NAMES ==========
    [Column("id")]
    public override string? Id { get; set; }

    [Column("user_name")]
    [StringLength(256)]
    public override string? UserName { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Column("normalized_user_name")]
    [StringLength(256)]
    public override string? NormalizedUserName { get; set; }

    [Column("email")]
    [StringLength(256)]
    public override string? Email { get; set; }

    [Column("normalized_email")]
    [StringLength(256)]
    public override string? NormalizedEmail { get; set; }

    [Column("email_confirmed")]
    public override bool EmailConfirmed { get; set; }

    [Column("password_hash")]
    public override string? PasswordHash { get; set; }

    [Column("security_stamp")]
    public override string? SecurityStamp { get; set; }

    [Column("concurrency_stamp")]
    public override string? ConcurrencyStamp { get; set; }

    [Column("phone_number")]
    [StringLength(50)]
    public override string? PhoneNumber { get; set; }

    [Column("phone_number_confirmed")]
    public override bool PhoneNumberConfirmed { get; set; }

    [Column("two_factor_enabled")]
    public override bool TwoFactorEnabled { get; set; }

    [Column("lockout_end")]
    public override DateTimeOffset? LockoutEnd { get; set; }

    [Column("lockout_enabled")]
    public override bool LockoutEnabled { get; set; }

    [Column("access_failed_count")]
    public override int AccessFailedCount { get; set; }



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
