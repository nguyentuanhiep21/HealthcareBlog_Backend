using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCareBlog_Backend.Controllers
{
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

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        /// <summary>
        /// Get user's nutrition data (profile + active session)
        /// </summary>
        [HttpGet("data")]
        public async Task<ActionResult<NutritionDataDto>> GetUserData()
        {
            try
            {
                var userId = GetUserId();
                var data = await _nutritionService.GetUserNutritionDataAsync(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving nutrition data", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new chat session (will delete old session and create new one)
        /// </summary>
        [HttpPost("session/new")]
        public async Task<ActionResult<ChatSessionDto>> CreateNewSession([FromBody] CreateNutritionProfileDto profileDto)
        {
            try
            {
                var userId = GetUserId();
                var session = await _nutritionService.CreateNewSessionAsync(userId, profileDto);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating new session", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active chat session
        /// </summary>
        [HttpGet("session/active")]
        public async Task<ActionResult<ChatSessionDto>> GetActiveSession()
        {
            try
            {
                var userId = GetUserId();
                var session = await _nutritionService.GetActiveSessionAsync(userId);
                
                if (session == null)
                {
                    return NotFound(new { message = "No active session found" });
                }

                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving active session", error = ex.Message });
            }
        }

        /// <summary>
        /// Save a chat message
        /// </summary>
        [HttpPost("message")]
        public async Task<ActionResult<ChatMessageDto>> SaveMessage([FromBody] SaveChatMessageDto dto)
        {
            try
            {
                var userId = GetUserId();
                var message = await _nutritionService.SaveMessageAsync(userId, dto);
                return Ok(message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving message", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user's nutrition profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<NutritionProfileDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _nutritionService.GetUserProfileAsync(userId);
                
                if (profile == null)
                {
                    return NotFound(new { message = "Profile not found" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving profile", error = ex.Message });
            }
        }
    }
}
