using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

/// <summary>
/// Toàn bộ filtering được thực thi TẠI DB qua LINQ → SQL.
/// Npgsql dịch:
///   m.SuitableFor.Contains(goal)  →  @goal = ANY(suitable_for)
///   EF.Functions.Random()         →  random()
///   .Take(count)                  →  LIMIT @count
/// Không load toàn bộ bảng meals lên server.
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
        var excludeList = excludeIds?.ToList() ?? [];

        return _context.Meals
            .Where(m =>
                m.MealType == mealType &&
                m.SuitableFor.Contains(goal) &&          // @goal = ANY(suitable_for)
                m.CaloriesPerServing >= caloriesMin &&
                m.CaloriesPerServing <= caloriesMax &&
                m.IsActive &&
                !excludeList.Contains(m.Id))             // NOT IN (@excludeIds)
            .OrderBy(_ => EF.Functions.Random())         // ORDER BY random()
            .Take(count)                                 // LIMIT @count
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
        var excludeList = excludeIds?.ToList() ?? [];

        return _context.Meals
            .Where(m =>
                m.MealType == mealType &&
                m.SuitableFor.Contains(goal) &&
                m.IsActive &&
                !excludeList.Contains(m.Id))
            .OrderBy(_ => EF.Functions.Random())
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}
