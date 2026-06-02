using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Exceptions;
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
///   Snack  5% ± 20% × 2  (mỗi snack ~5% tổng calo)
///
/// QUAN TRỌNG: Các query chạy TUẦN TỰ — EF Core DbContext KHÔNG thread-safe,
/// không dùng Task.WhenAll trên cùng 1 DbContext instance.
/// Mỗi query đã đẩy filter xuống DB (WHERE + ORDER BY RANDOM() + LIMIT).
/// </summary>
public class MealSuggestionService : IMealSuggestionService
{
    private readonly IMealSuggestionRepository _repo;

    // Tỷ lệ calo cho từng bữa
    private const double BreakfastRatio = 0.25;
    private const double LunchRatio     = 0.35;
    private const double DinnerRatio    = 0.30;
    private const double SnackRatio     = 0.05; // mỗi snack chiếm 5% (2 snack = 10%)

    // Tolerance khi tìm trong range calo
    private const double PrimaryTolerance  = 0.15; // ±15%
    private const double FallbackTolerance = 0.30; // ±30%

    public MealSuggestionService(IMealSuggestionRepository repo)
    {
        _repo = repo;
    }

    public async Task<DailyMealPlanDto> RecommendAsync(
        MealSuggestionRequestDto request)
    {
        if (request.CaloriesKcal <= 0)
            throw new BadRequestException("Calories target phải lớn hơn 0.");

        var validGoals = new[] { "Lose_Fat", "Gain_Muscle", "Maintain" };
        if (!validGoals.Contains(request.Goal))
            throw new BadRequestException($"Goal không hợp lệ: '{request.Goal}'. Hợp lệ: Lose_Fat, Gain_Muscle, Maintain.");

        int total = request.CaloriesKcal;

        // ── Tính target calo cho từng bữa ─────────────────────────────────────
        int breakfastTarget = (int)(total * BreakfastRatio);
        int lunchTarget     = (int)(total * LunchRatio);
        int dinnerTarget    = (int)(total * DinnerRatio);
        int snackTarget     = (int)(total * SnackRatio);

        // ── Chạy TUẦN TỰ (EF Core DbContext không thread-safe) ────────────────
        // Mỗi query đã filter hoàn toàn tại DB: WHERE + ORDER BY RANDOM() + LIMIT 1
        var breakfast = await PickMealAsync("breakfast", request.Goal, breakfastTarget, PrimaryTolerance, []);
        var lunch     = await PickMealAsync("lunch",     request.Goal, lunchTarget,     PrimaryTolerance, []);
        var dinner    = await PickMealAsync("dinner",    request.Goal, dinnerTarget,    PrimaryTolerance, []);
        var snack1    = await PickMealAsync("snack",     request.Goal, snackTarget,     PrimaryTolerance, []);

        // Snack 2: loại trừ id của snack 1 để không trùng
        var excludeSnack = snack1 is not null ? new[] { snack1.Id } : Array.Empty<int>();
        var snack2       = await PickMealAsync("snack", request.Goal, snackTarget, PrimaryTolerance, excludeSnack);

        // ── Kiểm tra bữa chính bắt buộc ───────────────────────────────────────
        if (breakfast is null || lunch is null || dinner is null)
        {
            var missing = string.Join(", ", new[]
            {
                breakfast is null ? "bữa sáng" : null,
                lunch     is null ? "bữa trưa" : null,
                dinner    is null ? "bữa tối"  : null,
            }.Where(x => x is not null));

            throw new BadRequestException($"Không tìm được món phù hợp cho: {missing}. Vui lòng kiểm tra dữ liệu bảng meals.");
        }

        // ── Build snacks list ──────────────────────────────────────────────────
        var snacks = new List<MealItemDto>();
        if (snack1 is not null) snacks.Add(MapToDto(snack1));
        if (snack2 is not null && (snack1 is null || snack2.Id != snack1.Id))
            snacks.Add(MapToDto(snack2));

        // ── Tính tổng macro thực tế ────────────────────────────────────────────
        var allMeals = new List<Meal?> { breakfast, lunch, dinner, snack1, snack2 }
            .Where(m => m is not null)
            .Cast<Meal>()
            .ToList();

        int    totalCal     = allMeals.Sum(m => m.CaloriesPerServing);
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
                TargetCalories   = total,
                TotalCalories    = totalCal,
                TotalProteinG    = Math.Round(totalProtein, 1),
                TotalCarbsG      = Math.Round(totalCarbs,   1),
                TotalFatG        = Math.Round(totalFat,     1),
                CaloriesCoverage = coverage,
                CoverageNote     = note,
            }
        };

        return plan;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Thử tìm với range hẹp (±15%) → mở rộng (±30%) → bỏ ràng buộc calo.
    /// </summary>
    private async Task<Meal?> PickMealAsync(
        string           mealType,
        string           goal,
        int              targetCal,
        double           tolerance,
        IEnumerable<int> excludeIds)
    {
        var exclude = excludeIds as int[] ?? excludeIds.ToArray();

        // Thử range hẹp ±15%
        int min = (int)(targetCal * (1 - tolerance));
        int max = (int)(targetCal * (1 + tolerance));
        var results = await _repo.GetRandomMealsAsync(mealType, goal, min, max, 1, exclude);
        if (results.Count > 0) return results[0];

        // Mở rộng ±30%
        min = (int)(targetCal * (1 - FallbackTolerance));
        max = (int)(targetCal * (1 + FallbackTolerance));
        results = await _repo.GetRandomMealsAsync(mealType, goal, min, max, 1, exclude);
        if (results.Count > 0) return results[0];

        // Fallback: bỏ ràng buộc calo, chỉ giữ mealType + goal
        results = await _repo.GetFallbackMealsAsync(mealType, goal, 1, exclude);
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
