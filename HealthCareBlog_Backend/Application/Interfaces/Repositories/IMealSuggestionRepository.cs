using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository gợi ý thực đơn.
/// Mỗi method thực thi 1 SELECT tại DB với WHERE + LIMIT — không load toàn bảng.
/// </summary>
public interface IMealSuggestionRepository
{
    /// <summary>
    /// Lấy ngẫu nhiên <paramref name="count"/> món ăn phù hợp điều kiện.
    /// Query chạy tại DB: WHERE meal_type + suitable_for + calories range + is_active.
    /// ORDER BY RANDOM() LIMIT <paramref name="count"/>.
    /// </summary>
    /// <param name="mealType">'breakfast' | 'lunch' | 'dinner' | 'snack'</param>
    /// <param name="goal">'Lose_Fat' | 'Gain_Muscle' | 'Maintain'</param>
    /// <param name="caloriesMin">Calo tối thiểu (kcal)</param>
    /// <param name="caloriesMax">Calo tối đa (kcal)</param>
    /// <param name="count">Số món cần lấy (LIMIT)</param>
    /// <param name="excludeIds">Danh sách ID đã chọn để tránh trùng lặp</param>
    Task<List<Meal>> GetRandomMealsAsync(
        string            mealType,
        string            goal,
        int               caloriesMin,
        int               caloriesMax,
        int               count,
        IEnumerable<int>? excludeIds = null);

    /// <summary>
    /// Lấy món fallback khi không tìm được với range chặt —
    /// chỉ filter theo meal_type + goal + is_active, bỏ ràng buộc calo.
    /// </summary>
    Task<List<Meal>> GetFallbackMealsAsync(
        string            mealType,
        string            goal,
        int               count,
        IEnumerable<int>? excludeIds = null);
}
