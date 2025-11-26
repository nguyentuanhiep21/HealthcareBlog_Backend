using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Users - Lưu thông tin người dùng của hệ thống
/// Chứa dữ liệu xác thực (từ IdentityUser), thông tin hồ sơ, sức khỏe,
/// trạng thái tài khoản và các liên kết tới bài viết, bình luận, nhóm, tin nhắn...
/// </summary>
[Table("users")]
public class ApplicationUser : IdentityUser
{
    // ========== BASIC INFORMATION ==========
    [Column("user_name")]
    [StringLength(256)]
    public override string? UserName { get; set; } // Tên đăng nhập/hiển thị

    [Column("email")]
    [StringLength(256)]
    public override string? Email { get; set; } // Địa chỉ email

    [Column("phone_number")]
    [StringLength(50)]
    public override string? PhoneNumber { get; set; } // Số điện thoại

    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; } // Tên đầy đủ

    // ========== PROFILE INFORMATION ==========
    [Column("bio")]
    [StringLength(500)]
    public string? Bio { get; set; } // Tiểu sử ngắn

    [Column("profile_picture")]
    [StringLength(500)]
    public string? ProfilePicture { get; set; } // URL ảnh đại diện

    // ========== HEALTH INFORMATION ==========
    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; } // Ngày sinh

    [Column("gender")]
    public Gender? Gender { get; set; } // Giới tính

    [Column("height")]
    [Precision(5, 2)]
    public decimal? Height { get; set; } // Chiều cao (cm)

    [Column("weight")]
    [Precision(5, 2)]
    public decimal? Weight { get; set; } // Cân nặng (kg)

    // ========== SYSTEM INFORMATION ==========
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian tạo tài khoản

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật cuối

    [Column("is_active")]
    public bool IsActive { get; set; } // Tài khoản đang hoạt động

    [Column("status")]
    public UserStatus Status { get; set; } = UserStatus.Active; // Trạng thái người dùng

    // ========== BAN/DEACTIVATION INFO ==========
    [Column("deactivated_at")]
    public DateTime? DeactivatedAt { get; set; } // Thời gian vô hiệu hóa

    [Column("deactivated_by")]
    [StringLength(450)]
    public string? DeactivatedBy { get; set; } // ID admin đã vô hiệu hóa

    [Column("deactivation_reason")]
    [StringLength(1000)]
    public string? DeactivationReason { get; set; } // Lý do vô hiệu hóa

    [Column("banned_at")]
    public DateTime? BannedAt { get; set; } // Thời gian bị cấm

    [Column("banned_by")]
    [StringLength(450)]
    public string? BannedBy { get; set; } // ID admin đã cấm

    [Column("ban_reason")]
    [StringLength(1000)]
    public string? BanReason { get; set; } // Lý do bị cấm

    [Column("ban_expires_at")]
    public DateTime? BanExpiresAt { get; set; } // Thời hạn hết cấm (null = vĩnh viễn)

    // ========== NAVIGATION PROPERTIES ==========
    // Bài viết, bình luận, lượt thích
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>(); // Bài viết của người dùng
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>(); // Bình luận của người dùng
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>(); // Lượt thích của người dùng

    // Follow
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>(); // Người user đang follow
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>(); // Người follow user

    // Block
    public virtual ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>(); // Người user đã chặn
    public virtual ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>(); // Người đã chặn user

    // Report
    public virtual ICollection<ReportedContent> Reports { get; set; } = new List<ReportedContent>(); // Báo cáo do user tạo

    // Health & Meal
    public virtual HealthProfile? HealthProfile { get; set; } // Hồ sơ sức khỏe
    public virtual ICollection<MealSuggestion> MealSuggestions { get; set; } = new List<MealSuggestion>(); // Gợi ý bữa ăn

    // Notifications
    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>(); // Thông báo nhận được

    // Admin relations
    [ForeignKey("DeactivatedBy")]
    public virtual ApplicationUser? DeactivatedByAdmin { get; set; } // Admin đã vô hiệu hóa

    [ForeignKey("BannedBy")]
    public virtual ApplicationUser? BannedByAdmin { get; set; } // Admin đã cấm

    // ========== COMPUTED PROPERTIES ==========
    [NotMapped]
    public int? Age
    {
        get
        {
            if (DateOfBirth == null) return null;
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age; // Tuổi người dùng
        }
    }

    [NotMapped]
    public decimal? BMI
    {
        get
        {
            if (Height == null || Weight == null || Height == 0) return null;
            var heightInMeters = Height.Value / 100;
            return Math.Round(Weight.Value / (heightInMeters * heightInMeters), 2); // Chỉ số khối cơ thể
        }
    }

    [NotMapped]
    public bool IsBanned => Status == UserStatus.Banned &&
                           (BanExpiresAt == null || BanExpiresAt > DateTime.UtcNow); // Người đang bị cấm

    [NotMapped]
    public bool IsDeactivated => Status == UserStatus.Deactivated; // Người đang bị vô hiệu hóa
}
