using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// Gợi ý thực đơn 1 ngày dựa trên nutrition targets từ HealthAssessment.
///
/// Phân bổ calo theo bữa:
///   Sáng  25% ± 15%
///   Trưa  35% ± 15%
///   Tối   30% ± 15%
///   Snack 10% ± 20%  (tối đa 2 snacks)
///
/// Mỗi slot gọi 1 DB query độc lập (WHERE + ORDER BY RANDOM() + LIMIT).
/// Nếu không tìm được trong range hẹp → fallback bỏ ràng buộc calo.
/// </summary>
public class MealSuggestionService : IMealSuggestionService
{
    private readonly IMealSuggestionRepository _repo;

    // Tỷ lệ calo cho từng bữa
    private const double BreakfastRatio = 0.25;
    private const double LunchRatio     = 0.35;
    private const double DinnerRatio    = 0.30;
    private const double SnackRatio     = 0.10;  // cho 1 snack; lấy 2 snack thì mỗi cái ~5%

    // Tolerance khi tìm trong range
    private const double PrimaryTolerance  = 0.15; // ±15%
    private const double FallbackTolerance = 0.30; // ±30% — mở rộng nếu không tìm được

    public MealSuggestionService(IMealSuggestionRepository repo)
    {
        _repo = repo;
    }

    public async Task<(DailyMealPlanDto? plan, string? error)> RecommendAsync(
        MealSuggestionRequestDto request)
    {
        if (request.CaloriesKcal <= 0)
            return (null, "Calories target phải lớn hơn 0.");

        var validGoals = new[] { "Lose_Fat", "Gain_Muscle", "Maintain" };
        if (!validGoals.Contains(request.Goal))
            return (null, $"Goal không hợp lệ: '{request.Goal}'. Hợp lệ: Lose_Fat, Gain_Muscle, Maintain.");

        int total = request.CaloriesKcal;

        // ── Tính target calo cho từng bữa ─────────────────────────────────
        int breakfastTarget = (int)(total * BreakfastRatio);
        int lunchTarget     = (int)(total * LunchRatio);
        int dinnerTarget    = (int)(total * DinnerRatio);
        int snackTarget     = (int)(total * SnackRatio / 2); // chia 2 snacks

        // ── Chạy 4 query song song tại DB ─────────────────────────────────
        var (bfTask, lunchTask, dinnerTask, snack1Task, snack2Task) = (
            PickMealAsync("breakfast", request.Goal, breakfastTarget, PrimaryTolerance, []),
            PickMealAsync("lunch",     request.Goal, lunchTarget,     PrimaryTolerance, []),
            PickMealAsync("dinner",    request.Goal, dinnerTarget,    PrimaryTolerance, []),
            PickMealAsync("snack",     request.Goal, snackTarget,     PrimaryTolerance, []),
            PickMealAsync("snack",     request.Goal, snackTarget,     PrimaryTolerance, [])
        );

        await Task.WhenAll(bfTask, lunchTask, dinnerTask, snack1Task, snack2Task);

        var breakfast = bfTask.Result;
        var lunch     = lunchTask.Result;
        var dinner    = dinnerTask.Result;
        var snack1    = snack1Task.Result;
        var snack2    = snack2Task.Result;

        // Loại bỏ snack trùng nhau
        if (snack1 is not null && snack2 is not null && snack1.Id == snack2.Id)
        {
            // Lấy snack2 mới, loại trừ id của snack1
            snack2 = await PickMealAsync("snack", request.Goal, snackTarget,
                                          PrimaryTolerance, [snack1.Id]);
        }

        // Kiểm tra bữa chính bắt buộc
        if (breakfast is null || lunch is null || dinner is null)
        {
            var missing = string.Join(", ", new[]
            {
                breakfast is null ? "bữa sáng" : null,
                lunch     is null ? "bữa trưa" : null,
                dinner    is null ? "bữa tối"  : null,
            }.Where(x => x is not null));

            return (null, $"Không tìm được món phù hợp cho: {missing}. Vui lòng kiểm tra dữ liệu bảng meals.");
        }

        // ── Build snacks list (1 hoặc 2 snacks) ───────────────────────────
        var snacks = new List<MealItemDto>();
        if (snack1 is not null) snacks.Add(MapToDto(snack1));
        if (snack2 is not null && (snack1 is null || snack2.Id != snack1.Id))
            snacks.Add(MapToDto(snack2));

        // ── Tính tổng macro thực tế ────────────────────────────────────────
        var allMeals = new List<Meal?> { breakfast, lunch, dinner, snack1, snack2 }
            .Where(m => m is not null)
            .Cast<Meal>()
            .ToList();

        int   totalCal     = allMeals.Sum(m => m.CaloriesPerServing);
        double totalProtein = (double)allMeals.Sum(m => m.ProteinG);
        double totalCarbs   = (double)allMeals.Sum(m => m.CarbsG);
        double totalFat     = (double)allMeals.Sum(m => m.FatG);

        int coverage = total > 0 ? (int)Math.Round((double)totalCal / total * 100) : 0;
        string note  = coverage switch
        {
            >= 95 and <= 105 => $"Đạt {coverage}% mục tiêu calo hôm nay ✓",
            > 105            => $"Vượt {coverage - 100}% so với mục tiêu calo",
            _                => $"Đạt {coverage}% mục tiêu calo hôm nay"
        };

        var plan = new DailyMealPlanDto
        {
            Breakfast = MapToDto(breakfast),
            Lunch     = MapToDto(lunch),
            Dinner    = MapToDto(dinner),
            Snacks    = snacks,
            Summary   = new MealSummaryDto
            {
                TargetCalories  = total,
                TotalCalories   = totalCal,
                TotalProteinG   = Math.Round(totalProtein, 1),
                TotalCarbsG     = Math.Round(totalCarbs,   1),
                TotalFatG       = Math.Round(totalFat,     1),
                CaloriesCoverage = coverage,
                CoverageNote     = note,
            }
        };

        return (plan, null);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Thử tìm với range hẹp trước; nếu không có → mở rộng tolerance → fallback.
    /// </summary>
    private async Task<Meal?> PickMealAsync(
        string mealType,
        string goal,
        int    targetCal,
        double tolerance,
        IEnumerable<int> excludeIds)
    {
        int min  = (int)(targetCal * (1 - tolerance));
        int max  = (int)(targetCal * (1 + tolerance));

        // Thử range hẹp
        var results = await _repo.GetRandomMealsAsync(mealType, goal, min, max, 1, excludeIds);
        if (results.Count > 0) return results[0];

        // Mở rộng tolerance lần 2
        min = (int)(targetCal * (1 - FallbackTolerance));
        max = (int)(targetCal * (1 + FallbackTolerance));
        results = await _repo.GetRandomMealsAsync(mealType, goal, min, max, 1, excludeIds);
        if (results.Count > 0) return results[0];

        // Fallback: bỏ ràng buộc calo hoàn toàn
        results = await _repo.GetFallbackMealsAsync(mealType, goal, 1, excludeIds);
        return results.Count > 0 ? results[0] : null;
    }

    private static MealItemDto MapToDto(Meal m) => new()
    {
        Id                 = m.Id,
        Name               = m.Name,
        NameEn             = m.NameEn,
        MealType           = m.MealType,
        CaloriesPerServing = m.CaloriesPerServing,
        ProteinG           = (double)Math.Round(m.ProteinG, 1),
        CarbsG             = (double)Math.Round(m.CarbsG,   1),
        FatG               = (double)Math.Round(m.FatG,     1),
        ServingSizeDesc    = m.ServingSizeDesc,
        Tags               = m.Tags,
        Description        = m.Description,
        ImageUrl           = m.ImageUrl,
    };
}
