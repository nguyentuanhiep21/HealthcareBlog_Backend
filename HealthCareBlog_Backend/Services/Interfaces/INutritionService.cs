using HealthCareBlog_Backend.Models.DTOs;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface INutritionService
    {
        // Profile Management
        Task<NutritionProfileDto?> GetUserProfileAsync(string userId);
        Task<NutritionProfileDto> CreateOrUpdateProfileAsync(string userId, CreateNutritionProfileDto dto);
        
        // Chat Session Management
        Task<NutritionDataDto> GetUserNutritionDataAsync(string userId);
        Task<ChatSessionDto> CreateNewSessionAsync(string userId, CreateNutritionProfileDto profileDto);
        Task<ChatSessionDto?> GetActiveSessionAsync(string userId);
        
        // Message Management
        Task<ChatMessageDto> SaveMessageAsync(string userId, SaveChatMessageDto dto);
        Task<List<ChatMessageDto>> GetSessionMessagesAsync(int sessionId);
    }
}
