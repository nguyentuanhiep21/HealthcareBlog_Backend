namespace HealthCareBlog_Backend.Models.DTOs
{
    // ── Request ──────────────────────────────────────────────────────────────
    public class HealthAssessRequestDto
    {
        /// <summary>Giới tính: "Male" | "Female"</summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>Tuổi (10–100)</summary>
        public int Age { get; set; }

        /// <summary>Chiều cao (cm, 100–250)</summary>
        public float Height { get; set; }

        /// <summary>Cân nặng (kg, 30–300)</summary>
        public float Weight { get; set; }

        /// <summary>Mục tiêu: "Gain_Muscle" | "Lose_Fat" | "Maintain"</summary>
        public string Goal { get; set; } = string.Empty;
    }

    // ── Response ─────────────────────────────────────────────────────────────
    public class NutritionTargetDto
    {
        public int CaloriesKcal { get; set; }
        public int ProteinG    { get; set; }
        public int CarbsG      { get; set; }
        public int FatG        { get; set; }
    }

    public class HealthAssessResultDto
    {
        public string Status      { get; set; } = "ok";
        public object? Input      { get; set; }
        public float   Bmi        { get; set; }
        public string  BmiCategory{ get; set; } = string.Empty;
        public int     HealthScore { get; set; }
        public NutritionTargetDto Nutrition { get; set; } = new();
        public string  Advice     { get; set; } = string.Empty;
    }

    public class HealthAssessErrorDto
    {
        public string Status  { get; set; } = "error";
        public string Message { get; set; } = string.Empty;
    }
}
