using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng MealSuggestions - Lưu trữ gợi ý bữa ăn từ AI
/// Dựa trên HealthProfile của user để AI tạo gợi ý phù hợp
/// Lưu trữ thông tin dinh dưỡng (calories, protein, carbs, fat...)
/// Phân loại theo loại bữa ăn (Breakfast, Lunch, Dinner, Snack)
/// </summary>
[Table("meal_suggestions")]
public partial class MealSuggestion
{
    [Key]
    [Column("suggestion_id")]
    public int SuggestionId { get; set; } // ID gợi ý

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người nhận gợi ý

    [Required]
    [Column("meal_type")]
    [StringLength(50)]
    public string MealType { get; set; } = null!; // Loại: Breakfast, Lunch, Dinner, Snack

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!; // Nội dung gợi ý từ AI

    [Column("calories")]
    public int Calories { get; set; } // Tổng calories

    [Column("nutrients")]
    public string? Nutrients { get; set; } // JSON object: {protein, carbs, fat, fiber...}

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo gợi ý

    [Column("ai_model_version")]
    [StringLength(50)]
    public string? AIModelVersion { get; set; } // Version của AI model (để tracking)

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
