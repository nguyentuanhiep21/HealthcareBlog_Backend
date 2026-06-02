using HealthCareBlog_Backend.Models.DTOs;

namespace HealthCareBlog_Backend.Services.Interfaces;

public interface IMealSuggestionService
{
    /// <summary>
    /// Gợi ý thực đơn 1 ngày dựa trên nutrition targets từ HealthAssessment.
    /// Trả về DailyMealPlanDto hoặc ném ra exception nếu không hợp lệ.
    /// </summary>
    Task<DailyMealPlanDto> RecommendAsync(MealSuggestionRequestDto request);
}
