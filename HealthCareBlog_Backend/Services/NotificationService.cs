using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Notifications;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDTO>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var notifications = await _context.Notifications
                .Include(n => n.Actor)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Type = n.Type.ToString().ToLower(),
                    Content = n.Content,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Actor = n.Actor != null ? new ActorDTO
                    {
                        Id = n.Actor.Id!,
                        FullName = n.Actor.FullName ?? "Unknown",
                        AvatarUrl = n.Actor.AvatarUrl
                    } : null,
                    PostId = n.PostId,
                    CommentId = n.CommentId
                })
                .ToListAsync();

            return notifications;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkAsReadAsync(string userId, int notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CreateNotificationAsync(string userId, string actorId, NotificationType type, string content, int? postId = null, int? commentId = null)
        {
            // Don't create notification if user is notifying themselves
            if (userId == actorId)
                return;

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

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
