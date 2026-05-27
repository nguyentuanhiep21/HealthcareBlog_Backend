using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface cho Nutrition — NutritionProfile, ChatSession, ChatMessage.
/// </summary>
public interface INutritionRepository
{
    Task<NutritionProfile?> GetProfileByUserIdAsync(string userId);
    Task AddProfileAsync(NutritionProfile profile);

    Task<NutritionChatSession?> GetSessionByProfileIdAsync(int profileId);
    Task<NutritionChatSession?> GetActiveSessionWithMessagesAsync(string userId);
    Task<NutritionChatSession?> GetSessionWithProfileAsync(string userId);
    Task AddSessionAsync(NutritionChatSession session);
    void RemoveSession(NutritionChatSession session);

    Task AddMessageAsync(NutritionChatMessage message);
    Task<List<NutritionChatMessage>> GetMessagesBySessionIdAsync(int sessionId);

    Task<int> SaveChangesAsync();
}
