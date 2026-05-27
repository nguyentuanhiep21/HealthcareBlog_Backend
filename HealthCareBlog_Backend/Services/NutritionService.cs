using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;
using System.Text.Json;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// NutritionService — business logic cho Nutrition module.
/// Không còn phụ thuộc ApplicationDbContext — dùng INutritionRepository.
/// </summary>
public class NutritionService : INutritionService
{
    private readonly INutritionRepository _nutritionRepository;

    public NutritionService(INutritionRepository nutritionRepository)
    {
        _nutritionRepository = nutritionRepository;
    }

    private static decimal CalculateBMI(decimal height, decimal weight)
        => Math.Round(weight / (height / 100 * height / 100), 1);

    private static string GetBMIStatus(decimal bmi)
    {
        if (bmi < 18.5m) return "Gầy";
        if (bmi < 25m) return "Bình thường";
        if (bmi < 30m) return "Thừa cân";
        return "Béo phì";
    }

    public async Task<NutritionProfileDto?> GetUserProfileAsync(string userId)
    {
        var profile = await _nutritionRepository.GetProfileByUserIdAsync(userId);
        if (profile == null) return null;
        return MapToDto(profile);
    }

    public async Task<NutritionProfileDto> CreateOrUpdateProfileAsync(string userId, CreateNutritionProfileDto dto)
    {
        var profile = await _nutritionRepository.GetProfileByUserIdAsync(userId);
        var bmi = CalculateBMI(dto.Height, dto.Weight);
        var bmiStatus = GetBMIStatus(bmi);

        if (profile == null)
        {
            profile = new NutritionProfile
            {
                UserId = userId,
                Gender = dto.Gender,
                Age = dto.Age,
                Height = dto.Height,
                Weight = dto.Weight,
                BMI = bmi,
                BMIStatus = bmiStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _nutritionRepository.AddProfileAsync(profile);
        }
        else
        {
            profile.Gender = dto.Gender;
            profile.Age = dto.Age;
            profile.Height = dto.Height;
            profile.Weight = dto.Weight;
            profile.BMI = bmi;
            profile.BMIStatus = bmiStatus;
            profile.UpdatedAt = DateTime.UtcNow;
        }

        await _nutritionRepository.SaveChangesAsync();
        return MapToDto(profile);
    }

    public async Task<NutritionDataDto> GetUserNutritionDataAsync(string userId)
    {
        var profile = await GetUserProfileAsync(userId);
        if (profile == null)
            return new NutritionDataDto { Profile = null, ActiveSession = null };

        var session = await GetActiveSessionAsync(userId);
        return new NutritionDataDto { Profile = profile, ActiveSession = session };
    }

    public async Task<ChatSessionDto> CreateNewSessionAsync(string userId, CreateNutritionProfileDto profileDto)
    {
        var profile = await CreateOrUpdateProfileAsync(userId, profileDto);

        var oldSession = await _nutritionRepository.GetSessionByProfileIdAsync(profile.Id);
        if (oldSession != null)
        {
            _nutritionRepository.RemoveSession(oldSession);
            await _nutritionRepository.SaveChangesAsync();
        }

        var newSession = new NutritionChatSession
        {
            NutritionProfileId = profile.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _nutritionRepository.AddSessionAsync(newSession);
        await _nutritionRepository.SaveChangesAsync();

        return new ChatSessionDto
        {
            Id = newSession.Id,
            NutritionProfileId = newSession.NutritionProfileId,
            CreatedAt = newSession.CreatedAt,
            UpdatedAt = newSession.UpdatedAt,
            Messages = new List<ChatMessageDto>()
        };
    }

    public async Task<ChatSessionDto?> GetActiveSessionAsync(string userId)
    {
        var session = await _nutritionRepository.GetActiveSessionWithMessagesAsync(userId);
        if (session == null) return null;
        return MapSessionToDto(session);
    }

    public async Task<ChatMessageDto> SaveMessageAsync(string userId, SaveChatMessageDto dto)
    {
        var session = await _nutritionRepository.GetSessionWithProfileAsync(userId)
            ?? throw new InvalidOperationException("No active chat session found for user");

        var message = new NutritionChatMessage
        {
            SessionId = session.Id,
            Role = dto.Role,
            Content = dto.Content,
            ParsedMealsJson = dto.ParsedMeals != null ? JsonSerializer.Serialize(dto.ParsedMeals) : null,
            CreatedAt = DateTime.UtcNow
        };

        await _nutritionRepository.AddMessageAsync(message);
        session.UpdatedAt = DateTime.UtcNow;
        await _nutritionRepository.SaveChangesAsync();

        return new ChatMessageDto
        {
            Id = message.Id,
            Role = message.Role,
            Content = message.Content,
            ParsedMeals = dto.ParsedMeals,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<List<ChatMessageDto>> GetSessionMessagesAsync(int sessionId)
    {
        var messages = await _nutritionRepository.GetMessagesBySessionIdAsync(sessionId);
        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            Role = m.Role,
            Content = m.Content,
            ParsedMeals = string.IsNullOrEmpty(m.ParsedMealsJson)
                ? null
                : JsonSerializer.Deserialize<List<MealDto>>(m.ParsedMealsJson),
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    // ========== Private helpers ==========

    private static NutritionProfileDto MapToDto(NutritionProfile profile) => new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        Gender = profile.Gender,
        Age = profile.Age,
        Height = profile.Height,
        Weight = profile.Weight,
        BMI = profile.BMI,
        BMIStatus = profile.BMIStatus,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    private static ChatSessionDto MapSessionToDto(NutritionChatSession session) => new()
    {
        Id = session.Id,
        NutritionProfileId = session.NutritionProfileId,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt,
        Messages = session.Messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            Role = m.Role,
            Content = m.Content,
            ParsedMeals = string.IsNullOrEmpty(m.ParsedMealsJson)
                ? null
                : JsonSerializer.Deserialize<List<MealDto>>(m.ParsedMealsJson),
            CreatedAt = m.CreatedAt
        }).ToList()
    };
}
