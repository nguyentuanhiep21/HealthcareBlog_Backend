using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng MealSuggestions - Lưu gợi ý bữa ăn do AI tạo dựa trên hồ sơ sức khỏe người dùng
/// Chứa dữ liệu đầu vào (age, height, weight, allergies, dietary restrictions)
/// Và phần phản hồi từ AI (ai_response) cùng các chỉ số dinh dưỡng khuyến nghị
/// </summary>
[Table("meal_suggestions")]
public class MealSuggestion
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID gợi ý

    // ========== USER INPUT DATA ==========
    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người dùng yêu cầu

    [Column("age")]
    public int Age { get; set; } // Tuổi khi tạo gợi ý

    [Column("gender")]
    public Gender Gender { get; set; } // Giới tính dùng cho tính toán dinh dưỡng

    [Column("height")]
    [Precision(5, 2)]
    public decimal Height { get; set; } // Chiều cao (cm)

    [Column("weight")]
    [Precision(5, 2)]
    public decimal Weight { get; set; } // Cân nặng (kg)

    [Column("bmi")]
    [Precision(5, 2)]
    public decimal BMI { get; set; } // BMI tính tại thời điểm tạo gợi ý

    [Column("activity_level")]
    public ActivityLevel? ActivityLevel { get; set; } // Mức độ vận động

    [Column("health_goal")]
    [StringLength(50)]
    public string? HealthGoal { get; set; } // Mục tiêu sức khỏe (giảm cân, tăng cơ...)

    [Column("dietary_restrictions")]
    [StringLength(500)]
    public string? DietaryRestrictions { get; set; } // Các hạn chế ăn uống (JSON hoặc text)

    [Column("allergies")]
    [StringLength(500)]
    public string? Allergies { get; set; } // Dị ứng thực phẩm

    [Column("meal_type")]
    [StringLength(20)]
    public string? MealType { get; set; } // Loại bữa (Breakfast, Lunch, Dinner, Snack)

    // ========== AI RESPONSE ==========
    [Column("ai_response")]
    [Required]
    public string AIResponse { get; set; } = string.Empty; // Nội dung gợi ý do AI trả về

    [Column("ai_model")]
    [StringLength(50)]
    public string? AIModel { get; set; } // Tên model/phiên bản AI tạo gợi ý

    [Column("calories_recommendation")]
    public int? CaloriesRecommendation { get; set; } // Lượng calo khuyến nghị

    [Column("protein_grams")]
    [Precision(5, 2)]
    public decimal? ProteinGrams { get; set; } // Lượng protein (gram)

    [Column("carbs_grams")]
    [Precision(5, 2)]
    public decimal? CarbsGrams { get; set; } // Lượng carbohydrate (gram)

    [Column("fat_grams")]
    [Precision(5, 2)]
    public decimal? FatGrams { get; set; } // Lượng chất béo (gram)

    // ========== METADATA ==========
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời điểm tạo gợi ý

    [Column("is_saved")]
    public bool IsSaved { get; set; } // Người dùng lưu gợi ý hay không

    [Column("rating")]
    public int? Rating { get; set; } // Đánh giá gợi ý bởi người dùng

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người dùng liên quan
}
