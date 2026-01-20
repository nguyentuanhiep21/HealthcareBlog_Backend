using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("nutrition_profile")]
public class NutritionProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty; // Nam, Nữ, Khác

    [Required]
    public int Age { get; set; }

    [Required]
    public decimal Height { get; set; } // cm

    [Required]
    public decimal Weight { get; set; } // kg

    [Required]
    public decimal BMI { get; set; }

    [MaxLength(50)]
    public string BMIStatus { get; set; } = string.Empty; // Gầy, Bình thường, Thừa cân, Béo phì

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property - Only one active session
    public NutritionChatSession? ActiveChatSession { get; set; }
}

