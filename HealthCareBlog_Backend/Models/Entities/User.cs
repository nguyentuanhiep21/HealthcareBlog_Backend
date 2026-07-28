using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Users - Kế thừa IdentityUser để tạo liên kết với các bảng khác trong hệ thống
/// </summary>
[Table("users")]
public class User : IdentityUser
{
    // ========== OVERRIDE IDENTITYUSER PROPERTIES TO CONTROL COLUMN NAMES ==========
    [Column("id")]
    public override string Id { get; set; } = "";

    [Column("user_name")]
    [StringLength(256)]
    public override string? UserName { get; set; }

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

    // ========== CUSTOM PROPERTIES ==========
    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Column("first_name")]
    [StringLength(50)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(50)]
    public string? LastName { get; set; }

    [Column("bio")]
    [StringLength(500)]
    public string? Bio { get; set; }

    [Column("avatar_url")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [Column("banner_url")]
    [StringLength(500)]
    public string? BannerUrl { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    [Column("follower_count")]
    public int FollowerCount { get; set; } = 0;

    [Column("following_count")]
    public int FollowingCount { get; set; } = 0;

    [Column("post_count")]
    public int PostCount { get; set; } = 0;

    [Column("is_locked")]
    public bool IsLocked { get; set; } = false;

    [Column("locked_at")]
    public DateTime? LockedAt { get; set; }

    [Column("unlock_date")]
    public DateTime? UnlockDate { get; set; }

    [Column("lock_reason")]
    [StringLength(500)]
    public string? LockReason { get; set; }

    // ========== NAVIGATION PROPERTIES ==========
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<LikePost> LikePosts { get; set; } = new List<LikePost>();
    public virtual ICollection<LikeComment> LikeComments { get; set; } = new List<LikeComment>();
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
    public virtual ICollection<ReportedContent> Reports { get; set; } = new List<ReportedContent>();
    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
}
