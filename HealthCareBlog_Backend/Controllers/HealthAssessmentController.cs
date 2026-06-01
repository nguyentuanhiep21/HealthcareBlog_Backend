using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// API đánh giá sức khỏe dựa trên chỉ số cơ thể.
/// Sử dụng công thức dinh dưỡng khoa học (Mifflin-St Jeor BMR + macro ratios).
/// Endpoint này KHÔNG yêu cầu đăng nhập — có thể dùng công khai.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthAssessmentController : ControllerBase
{
    private readonly IHealthAssessmentService _assessmentService;

    public HealthAssessmentController(IHealthAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    /// <summary>
    /// Đánh giá sức khỏe và tính toán mục tiêu dinh dưỡng cá nhân.
    /// </summary>
    /// <remarks>
    /// **gender:** "Male" | "Female"
    ///
    /// **goal:** "Gain_Muscle" | "Lose_Fat" | "Maintain"
    ///
    /// **Ví dụ request:**
    /// ```json
    /// {
    ///   "gender": "Male",
    ///   "age": 25,
    ///   "height": 175,
    ///   "weight": 70,
    ///   "goal": "Gain_Muscle"
    /// }
    /// ```
    /// </remarks>
    [HttpPost("assess")]
    [ProducesResponseType(typeof(HealthAssessResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthAssessErrorDto), StatusCodes.Status400BadRequest)]
    public ActionResult Assess([FromBody] HealthAssessRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new HealthAssessErrorDto { Message = "Request không hợp lệ." });

        var (result, error) = _assessmentService.Assess(request);

        if (error is not null)
            return BadRequest(new HealthAssessErrorDto { Message = error });

        return Ok(result);
    }
}
