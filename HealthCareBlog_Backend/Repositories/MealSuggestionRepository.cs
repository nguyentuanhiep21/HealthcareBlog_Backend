using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// Filtering thực thi TẠI DB qua raw SQL (PostgreSQL).
/// Dùng raw SQL để đảm bảo ORDER BY RANDOM() và ANY() hoạt động chính xác với Npgsql.
/// Mỗi query: WHERE meal_type + suitable_for + calories + is_active + LIMIT → không load toàn bảng.
/// </summary>
public class MealSuggestionRepository : IMealSuggestionRepository
{
    private readonly ApplicationDbContext _context;

    public MealSuggestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<List<Meal>> GetRandomMealsAsync(
        string            mealType,
        string            goal,
        int               caloriesMin,
        int               caloriesMax,
        int               count,
        IEnumerable<int>? excludeIds = null)
    {
        var exclude = excludeIds?.ToArray() ?? [];

        // Dùng LINQ — Npgsql 8 dịch Contains(string) trên string[] → @goal = ANY(col)
        // và .OrderBy(r => EF.Functions.Random()) → ORDER BY random()
        return _context.Meals
            .Where(m =>
                m.MealType == mealType &&
                m.SuitableFor.Any(s => s == goal) &&
                m.CaloriesPerServing >= caloriesMin &&
                m.CaloriesPerServing <= caloriesMax &&
                m.IsActive &&
                !exclude.Contains(m.Id))
            .OrderBy(m => EF.Functions.Random())
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<Meal>> GetFallbackMealsAsync(
        string            mealType,
        string            goal,
        int               count,
        IEnumerable<int>? excludeIds = null)
    {
        var exclude = excludeIds?.ToArray() ?? [];

        return _context.Meals
            .Where(m =>
                m.MealType == mealType &&
                m.SuitableFor.Any(s => s == goal) &&
                m.IsActive &&
                !exclude.Contains(m.Id))
            .OrderBy(m => EF.Functions.Random())
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}
