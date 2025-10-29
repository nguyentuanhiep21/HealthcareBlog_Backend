using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng HealthProfiles - Quản lý thông tin sức khỏe chi tiết của người dùng
/// Lưu trữ chiều cao, cân nặng, tuổi, BMI, mức độ hoạt động
/// Là cơ sở để AI tạo gợi ý bữa ăn phù hợp
/// Mỗi user chỉ có 1 health profile (one-to-one relationship)
/// </summary>
[Table("health_profiles")]
public partial class HealthProfile
{
    [Key]
    [Column("health_profile_id")]
    public int HealthProfileId { get; set; } // ID hồ sơ sức khỏe

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người dùng

    [Column("height", TypeName = "decimal(5,2)")]
    public decimal Height { get; set; } // Chiều cao (cm)

    [Column("weight", TypeName = "decimal(5,2)")]
    public decimal Weight { get; set; } // Cân nặng (kg)

    [Column("age")]
    public int Age { get; set; } // Tuổi

    [Column("bmi", TypeName = "decimal(5,2)")]
    public decimal BMI { get; set; } // Chỉ số BMI (tính từ height và weight)

    [Column("activity_level")]
    [StringLength(50)]
    public string? ActivityLevel { get; set; } // Mức độ hoạt động: Sedentary, Light, Moderate, Active, VeryActive

    [Column("dietary_restrictions")]
    public string? DietaryRestrictions { get; set; } // JSON array: ăn chay, dị ứng, không ăn gì...

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Thời gian cập nhật cuối

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người dùng sở hữu hồ sơ
}
