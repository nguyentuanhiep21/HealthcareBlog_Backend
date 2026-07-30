using HealthCareBlog_Backend.Models.DTOs.Notifications;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Interfaces;

/// <summary>
/// Repository interface cho Notification entity.
/// </summary>
public interface INotificationRepository
{
    Task<List<NotificationDTO>> GetUserNotificationsDTOAsync(string userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(string userId);
    Task<Notification?> GetByIdForUserAsync(int notificationId, string userId);
    Task<NotificationDTO?> GetNotificationDTOByIdAsync(int notificationId);
    Task<List<Notification>> GetUnreadByUserIdAsync(string userId);
    Task AddAsync(Notification notification);
    Task<int> SaveChangesAsync();
}
