using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("nutrition_chat_session")]
public class NutritionChatSession
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NutritionProfileId { get; set; }

    [ForeignKey(nameof(NutritionProfileId))]
    public NutritionProfile NutritionProfile { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<NutritionChatMessage> Messages { get; set; } = new List<NutritionChatMessage>();
}

