using HealthCareBlog_Backend.Models.DTOs.Notifications;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> MarkAsReadAsync(string userId, int notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task CreateNotificationAsync(string userId, string actorId, NotificationType type, string content, int? postId = null, int? commentId = null);
    }
}
