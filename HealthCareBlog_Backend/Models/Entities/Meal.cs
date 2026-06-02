using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng món ăn tĩnh — dữ liệu dinh dưỡng chuẩn Việt Nam.
/// Bảng đã được tạo sẵn trong DB (không qua EF Migration).
/// </summary>
[Table("meals")]
public class Meal
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("name_en")]
    public string? NameEn { get; set; }

    /// <summary>'breakfast' | 'lunch' | 'dinner' | 'snack'</summary>
    [Required]
    [MaxLength(20)]
    [Column("meal_type")]
    public string MealType { get; set; } = string.Empty;

    [Column("calories_per_serving")]
    public int CaloriesPerServing { get; set; }

    [Column("protein_g")]
    public decimal ProteinG { get; set; }

    [Column("carbs_g")]
    public decimal CarbsG { get; set; }

    [Column("fat_g")]
    public decimal FatG { get; set; }

    [MaxLength(150)]
    [Column("serving_size_desc")]
    public string ServingSizeDesc { get; set; } = string.Empty;

    /// <summary>
    /// PostgreSQL varchar[] — 'Lose_Fat' | 'Gain_Muscle' | 'Maintain'.
    /// Query: WHERE @goal = ANY(suitable_for)
    /// </summary>
    [Column("suitable_for", TypeName = "varchar[]")]
    public string[] SuitableFor { get; set; } = [];

    /// <summary>PostgreSQL varchar[] — nhãn phân loại món ăn.</summary>
    [Column("tags", TypeName = "varchar[]")]
    public string[] Tags { get; set; } = [];

    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(500)]
    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
