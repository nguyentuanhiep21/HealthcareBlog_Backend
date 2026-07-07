using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Notifications;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsDTOAsync(string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        return await _context.Notifications
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
    }

    public Task<int> GetUnreadCountAsync(string userId)
        => _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task<Notification?> GetByIdForUserAsync(int notificationId, string userId)
        => await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

    public async Task<List<Notification>> GetUnreadByUserIdAsync(string userId)
        => await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

    public async Task AddAsync(Notification notification)
        => await _context.Notifications.AddAsync(notification);

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
