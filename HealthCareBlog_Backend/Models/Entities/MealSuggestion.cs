using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("meal_suggestions")]
public partial class MealSuggestion
{
    [Key]
    [Column("suggestion_id")]
    public int SuggestionId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Required]
    [Column("meal_type")]
    [StringLength(50)]
    public string MealType { get; set; } = null!;

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("calories")]
    public int Calories { get; set; }

    [Column("nutrients")]
    public string? Nutrients { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("ai_model_version")]
    [StringLength(50)]
    public string? AIModelVersion { get; set; }

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
