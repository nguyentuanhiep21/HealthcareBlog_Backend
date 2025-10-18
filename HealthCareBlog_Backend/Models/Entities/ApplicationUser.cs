using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Users - Quản lý thông tin người dùng
/// Kế thừa từ IdentityUser để sử dụng hệ thống authentication có sẵn
/// Lưu trữ thông tin cá nhân, thông tin sức khỏe cơ bản của người dùng
/// </summary>
[Table("users")]
public partial class ApplicationUser : IdentityUser
{
    [Column("user_id")]
    public override string Id { get; set; } = null!; // ID người dùng (GUID)

    [Column("user_name")]
    public override string? UserName { get; set; } // Tên đăng nhập

    [Column("email")]
    public override string? Email { get; set; } // Email người dùng

    [Column("phone_number")]
    public override string? PhoneNumber { get; set; } // Số điện thoại

    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; } // Họ và tên đầy đủ

    [Column("bio")]
    [StringLength(500)]
    public string? Bio { get; set; } // Giới thiệu bản thân ngắn gọn

    [Column("profile_picture")]
    public string? ProfilePicture { get; set; } // URL ảnh đại diện

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; } // Ngày sinh

    [Column("height")]
    public decimal? Height { get; set; } // Chiều cao (cm)

    [Column("weight")]
    public decimal? Weight { get; set; } // Cân nặng (kg)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo tài khoản

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; } // Lần đăng nhập cuối cùng

    // Navigation Properties
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public virtual HealthProfile? HealthProfile { get; set; }
    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
    public virtual ICollection<ReportedContent> Reports { get; set; } = new List<ReportedContent>();
    public virtual ICollection<MealSuggestion> MealSuggestions { get; set; } = new List<MealSuggestion>();
}
