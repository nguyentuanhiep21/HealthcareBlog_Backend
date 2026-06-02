using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// API gợi ý thực đơn 1 ngày dựa trên nutrition targets.
/// Endpoint PUBLIC — không yêu cầu đăng nhập.
/// Mỗi lần gọi trả về combo món ăn khác nhau (ORDER BY RANDOM() tại DB).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MealSuggestionController : ControllerBase
{
    private readonly IMealSuggestionService _service;

    public MealSuggestionController(IMealSuggestionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gợi ý thực đơn 1 ngày (sáng + trưa + tối + snack).
    /// </summary>
    /// <remarks>
    /// Truyền vào output từ HealthAssessment API:
    ///
    /// ```json
    /// {
    ///   "caloriesKcal": 2200,
    ///   "proteinG": 165,
    ///   "carbsG": 248,
    ///   "fatG": 73,
    ///   "goal": "Gain_Muscle"
    /// }
    /// ```
    ///
    /// **goal:** "Lose_Fat" | "Gain_Muscle" | "Maintain"
    /// </remarks>
    [HttpPost("recommend")]
    [ProducesResponseType(typeof(DailyMealPlanDto),        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MealSuggestionErrorDto),  StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Recommend([FromBody] MealSuggestionRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new MealSuggestionErrorDto { Message = "Request không hợp lệ." });

        var (plan, error) = await _service.RecommendAsync(request);

        if (error is not null)
            return BadRequest(new MealSuggestionErrorDto { Message = error });

        return Ok(plan);
    }
}
