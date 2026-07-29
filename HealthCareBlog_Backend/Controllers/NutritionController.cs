using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
namespace HealthCareBlog_Backend.Controllers;

[ApiController]
[Route("api/nutrition")]
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

}
