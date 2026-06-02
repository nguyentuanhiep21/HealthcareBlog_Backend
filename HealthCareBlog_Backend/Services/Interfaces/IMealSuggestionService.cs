using HealthCareBlog_Backend.Models.DTOs;

namespace HealthCareBlog_Backend.Services.Interfaces;

public interface IMealSuggestionService
{
    /// <summary>
    /// Gợi ý thực đơn 1 ngày dựa trên nutrition targets từ HealthAssessment.
    /// Trả về null nếu DB không đủ dữ liệu phù hợp.
    /// </summary>
    Task<(DailyMealPlanDto? plan, string? error)> RecommendAsync(MealSuggestionRequestDto request);
}
