namespace HealthCareBlog_Backend.Models.DTOs;

// ── Request ──────────────────────────────────────────────────────────────────

/// <summary>
/// Input cho endpoint POST /api/MealSuggestion/recommend.
/// Nhận trực tiếp output từ HealthAssessment API.
/// </summary>
public class MealSuggestionRequestDto
{
    /// <summary>Tổng calo cần nạp trong ngày (kcal).</summary>
    public int CaloriesKcal { get; set; }

    /// <summary>Protein mục tiêu (g/ngày).</summary>
    public int ProteinG { get; set; }

    /// <summary>Carbs mục tiêu (g/ngày).</summary>
    public int CarbsG { get; set; }

    /// <summary>Fat mục tiêu (g/ngày).</summary>
    public int FatG { get; set; }

    /// <summary>"Lose_Fat" | "Gain_Muscle" | "Maintain"</summary>
    public string Goal { get; set; } = string.Empty;
}

// ── Response ─────────────────────────────────────────────────────────────────

/// <summary>Thông tin 1 món ăn trong kết quả gợi ý.</summary>
public class MealItemDto
{
    public int    Id                 { get; set; }
    public string Name               { get; set; } = string.Empty;
    public string? NameEn            { get; set; }
    public string MealType           { get; set; } = string.Empty;
    public int    CaloriesPerServing { get; set; }
    public double ProteinG           { get; set; }
    public double CarbsG             { get; set; }
    public double FatG               { get; set; }
    public string ServingSizeDesc    { get; set; } = string.Empty;
    public string[] Tags             { get; set; } = [];
    public string? Description       { get; set; }
    public string? ImageUrl          { get; set; }
}

/// <summary>Tóm tắt tổng macro thực tế so với mục tiêu.</summary>
public class MealSummaryDto
{
    public int    TargetCalories  { get; set; }
    public int    TotalCalories   { get; set; }
    public double TotalProteinG   { get; set; }
    public double TotalCarbsG     { get; set; }
    public double TotalFatG       { get; set; }

    /// <summary>Phần trăm đạt mục tiêu calo (0–100+).</summary>
    public int CaloriesCoverage   { get; set; }

    /// <summary>Ghi chú ngắn: "Đạt 97% mục tiêu calo hôm nay"</summary>
    public string CoverageNote    { get; set; } = string.Empty;
}

/// <summary>
/// Response đầy đủ: 1 ngày ăn gồm sáng + trưa + tối + snack.
/// Mỗi lần gọi API sẽ trả về combo khác nhau (ORDER BY RANDOM() tại DB).
/// </summary>
public class DailyMealPlanDto
{
    public MealItemDto  Breakfast { get; set; } = null!;
    public MealItemDto  Lunch     { get; set; } = null!;
    public MealItemDto  Dinner    { get; set; } = null!;

    /// <summary>1–2 bữa phụ trong ngày.</summary>
    public List<MealItemDto> Snacks { get; set; } = [];

    public MealSummaryDto Summary { get; set; } = null!;
}

/// <summary>Response khi không tìm được đủ món phù hợp.</summary>
public class MealSuggestionErrorDto
{
    public string Status  { get; set; } = "error";
    public string Message { get; set; } = string.Empty;
}
