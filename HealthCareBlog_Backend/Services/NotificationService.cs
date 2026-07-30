using HealthCareBlog_Backend.Hubs;
using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Notifications;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// NotificationService — business logic cho Notification module.
/// Không còn phụ thuộc ApplicationDbContext — dùng INotificationRepository.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        => await _notificationRepository.GetUserNotificationsDTOAsync(userId, page, pageSize);

    public async Task<int> GetUnreadCountAsync(string userId)
        => await _notificationRepository.GetUnreadCountAsync(userId);

    public async Task<bool> MarkAsReadAsync(string userId, int notificationId)
    {
        var notification = await _notificationRepository.GetByIdForUserAsync(notificationId, userId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _notificationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
        foreach (var n in notifications)
            n.IsRead = true;

        await _notificationRepository.SaveChangesAsync();
        return true;
    }

    public async Task CreateNotificationAsync(
        string userId, string actorId, NotificationType type,
        string content, int? postId = null, int? commentId = null)
    {
        // Không tạo notification nếu user tự notify chính mình
        if (userId == actorId) return;

        var notification = new Notification
        {
            UserId = userId,
            ActorId = actorId,
            Type = type,
            Content = content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            PostId = postId,
            CommentId = commentId
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        // Push notification to client via SignalR
        var notificationDto = await _notificationRepository.GetNotificationDTOByIdAsync(notification.Id);
        if (notificationDto != null)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationDto);
        }
    }
}
