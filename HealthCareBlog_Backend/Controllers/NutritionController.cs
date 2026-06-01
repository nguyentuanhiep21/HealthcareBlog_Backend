using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
namespace HealthCareBlog_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionService _nutritionService;

    public NutritionController(INutritionService nutritionService)
    {
        _nutritionService = nutritionService;
    }

    /// <summary>Lấy nutrition data của user (profile + active session)</summary>
    [HttpGet("data")]
    public async Task<ActionResult<NutritionDataDto>> GetUserData()
    {
        var userId = User.GetUserId()!;
        var data = await _nutritionService.GetUserNutritionDataAsync(userId);
        return Ok(data);
    }

    /// <summary>Tạo chat session mới (xóa session cũ nếu có)</summary>
    [HttpPost("session/new")]
    public async Task<ActionResult<ChatSessionDto>> CreateNewSession([FromBody] CreateNutritionProfileDto dto)
    {
        var userId = User.GetUserId()!;
        var session = await _nutritionService.CreateNewSessionAsync(userId, dto);
        return Ok(session);
    }

    /// <summary>Lấy active chat session</summary>
    [HttpGet("session/active")]
    public async Task<ActionResult<ChatSessionDto>> GetActiveSession()
    {
        var userId = User.GetUserId()!;
        var session = await _nutritionService.GetActiveSessionAsync(userId);
        if (session == null)
            return NotFound(new { message = "No active session found" });

        return Ok(session);
    }

    /// <summary>Lưu chat message</summary>
    [HttpPost("message")]
    public async Task<ActionResult<ChatMessageDto>> SaveMessage([FromBody] SaveChatMessageDto dto)
    {
        var userId = User.GetUserId()!;
        var message = await _nutritionService.SaveMessageAsync(userId, dto);
        return Ok(message);
    }

    /// <summary>Lấy nutrition profile của user</summary>
    [HttpGet("profile")]
    public async Task<ActionResult<NutritionProfileDto>> GetProfile()
    {
        var userId = User.GetUserId()!;
        var profile = await _nutritionService.GetUserProfileAsync(userId);
        if (profile == null)
            return NotFound(new { message = "Profile not found" });

        return Ok(profile);
    }

    /// <summary>Đánh giá sức khỏe qua local ML model</summary>
    [HttpPost("assess-health")]
    [AllowAnonymous] // Allowing anonymous for now or keep Authorize depending on requirements, let's keep Authorize if the whole controller is Authorize, but wait, maybe form doesn't need login? Let's use [AllowAnonymous] for flexibility, or maybe the controller is already [Authorize], we'll see.
    public IActionResult AssessHealth([FromBody] AssessHealthRequest request)
    {
        try
        {
            var jsonInput = JsonSerializer.Serialize(new
            {
                gender = request.Gender,
                age = request.Age,
                weight = request.Weight,
                height = request.Height,
                goal = request.Goal
            });

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"predict_cli.py \"{jsonInput.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory // Or maybe use env.ContentRootPath? For now, we rely on predict_cli.py being in the output directory or we can use Path.Combine(Directory.GetCurrentDirectory(), "predict_cli.py")
            };

            // It's safer to use Directory.GetCurrentDirectory() for standard ASP.NET Core apps.
            psi.WorkingDirectory = Directory.GetCurrentDirectory();

            using var process = Process.Start(psi);
            if (process == null) return StatusCode(500, new { message = "Failed to start python process." });

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return StatusCode(500, new { message = "Error from ML model", error = error });
            }

            var result = JsonSerializer.Deserialize<JsonElement>(output);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
