using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("health_profiles")]
public partial class HealthProfile
{
    [Key]
    [Column("health_profile_id")]
    public int HealthProfileId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Column("height", TypeName = "decimal(5,2)")]
    public decimal Height { get; set; }

    [Column("weight", TypeName = "decimal(5,2)")]
    public decimal Weight { get; set; }

    [Column("age")]
    public int Age { get; set; }

    [Column("bmi", TypeName = "decimal(5,2)")]
    public decimal BMI { get; set; }

    [Column("activity_level")]
    [StringLength(50)]
    public string? ActivityLevel { get; set; }

    [Column("dietary_restrictions")]
    public string? DietaryRestrictions { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
