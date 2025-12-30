namespace HealthCareBlog_Backend.Models.DTOs
{
    public class CreateNutritionProfileDto
    {
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }

    public class NutritionProfileDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal BMI { get; set; }
        public string BMIStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
