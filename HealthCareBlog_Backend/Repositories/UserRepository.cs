using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// EF Core implementation của IUserRepository.
/// Tập trung toàn bộ raw EF queries liên quan User,
/// tách biệt khỏi Identity operations (UserManager).
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========== READ ==========

    public async Task<User?> GetByIdWithFollowersAsync(string userId)
        => await _context.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<List<User>> GetPagedAsync(int page, int pageSize, string? searchQuery = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var lower = searchQuery.ToLower();
            query = query.Where(u =>
                u.FullName!.ToLower().Contains(lower) ||
                u.Email!.ToLower().Contains(lower) ||
                u.UserName!.ToLower().Contains(lower));
        }

        return await query
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<SuggestedUserDTO>> GetSuggestedUsersAsync(string? currentUserId)
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        var adminUserIds = adminRole != null
            ? await _context.UserRoles
                .Where(ur => ur.RoleId == adminRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync()
            : new List<string>();

        return await _context.Users
            .Include(u => u.Followers)
            .Where(u => !adminUserIds.Contains(u.Id))
            .OrderByDescending(u => u.Followers.Count)
            .Take(3)
            .Select(u => new SuggestedUserDTO
            {
                Id = u.Id!,
                FullName = u.FullName ?? "",
                AvatarUrl = u.AvatarUrl,
                FollowerCount = u.Followers.Count,
                IsFollowing = currentUserId != null && u.Followers.Any(f => f.FollowerId == currentUserId)
            })
            .ToListAsync();
    }

    // ========== ADMIN STATS ==========

    public Task<int> CountUsersAsync() => _context.Users.CountAsync();
    public Task<int> CountLockedUsersAsync() => _context.Users.CountAsync(u => u.IsLocked);
    public Task<int> CountPostsAsync() => _context.Posts.CountAsync();
    public Task<int> CountCommentsAsync() => _context.Comments.CountAsync();
    public Task<int> CountPendingReportsAsync() => _context.Reports.CountAsync(r => r.Status == "Pending");
    public Task<int> CountPostsTodayAsync(DateTime today) => _context.Posts.CountAsync(p => p.CreatedAt >= today);
    public Task<int> CountReportsByContentTypeAsync(string contentType) => _context.Reports.CountAsync(r => r.ContentType == contentType);
    public Task<int> CountReportsByStatusAsync(string status) => _context.Reports.CountAsync(r => r.Status == status);

    // ========== DELETE USER CASCADE ==========

    public async Task<List<Post>> GetUserPostsWithDetailsAsync(string userId)
        => await _context.Posts
            .Where(p => p.UserId == userId)
            .ToListAsync();

    public async Task RemovePostLikesByPostIdsAsync(IEnumerable<int> postIds)
    {
        var likes = await _context.LikePosts
            .Where(l => postIds.Contains(l.PostId))
            .ToListAsync();
        if (likes.Any()) _context.LikePosts.RemoveRange(likes);
    }

    public async Task RemoveCommentLikesByCommentIdsAsync(IEnumerable<int> commentIds)
    {
        var likes = await _context.LikeComments
            .Where(l => commentIds.Contains(l.CommentId))
            .ToListAsync();
        if (likes.Any()) _context.LikeComments.RemoveRange(likes);
    }

    public async Task RemoveNotificationsByCommentIdsAsync(IEnumerable<int> commentIds)
    {
        var notifications = await _context.Notifications
            .Where(n => n.CommentId != null && commentIds.Contains(n.CommentId.Value))
            .ToListAsync();
        if (notifications.Any()) _context.Notifications.RemoveRange(notifications);
    }

    public async Task RemoveCommentsByPostIdsAsync(IEnumerable<int> postIds)
    {
        var comments = await _context.Comments
            .Where(c => postIds.Contains(c.PostId))
            .ToListAsync();
        if (comments.Any()) _context.Comments.RemoveRange(comments);
    }

    public async Task RemoveNotificationsByPostIdsAsync(IEnumerable<int> postIds)
    {
        var notifications = await _context.Notifications
            .Where(n => n.PostId != null && postIds.Contains(n.PostId.Value))
            .ToListAsync();
        if (notifications.Any()) _context.Notifications.RemoveRange(notifications);
    }

    public async Task RemovePostsByUserIdAsync(string userId)
    {
        var posts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
        if (posts.Any()) _context.Posts.RemoveRange(posts);
    }

    public async Task<List<int>> GetCommentIdsByUserIdAsync(string userId)
        => await _context.Comments
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToListAsync();

    public async Task RemoveCommentsByUserIdAsync(string userId)
    {
        var comments = await _context.Comments.Where(c => c.UserId == userId).ToListAsync();
        if (comments.Any()) _context.Comments.RemoveRange(comments);
    }

    public async Task RemovePostLikesByUserIdAsync(string userId)
    {
        var likes = await _context.LikePosts.Where(l => l.UserId == userId).ToListAsync();
        if (likes.Any()) _context.LikePosts.RemoveRange(likes);
    }

    public async Task RemoveCommentLikesByUserIdAsync(string userId)
    {
        var likes = await _context.LikeComments.Where(l => l.UserId == userId).ToListAsync();
        if (likes.Any()) _context.LikeComments.RemoveRange(likes);
    }

    public async Task RemoveFollowsByUserIdAsync(string userId)
    {
        var follows = await _context.Follows
            .Where(f => f.FollowerId == userId || f.FollowingId == userId)
            .ToListAsync();
        if (follows.Any()) _context.Follows.RemoveRange(follows);
    }

    public async Task RemoveSavedPostsByUserIdAsync(string userId)
    {
        var saved = await _context.SavedPosts.Where(sp => sp.UserId == userId).ToListAsync();
        if (saved.Any()) _context.SavedPosts.RemoveRange(saved);
    }

    public async Task RemoveNotificationsByUserIdAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId || n.ActorId == userId)
            .ToListAsync();
        if (notifications.Any()) _context.Notifications.RemoveRange(notifications);
    }

    public async Task ClearReporterReferenceAsync(string userId)
    {
        var reports = await _context.ReportedContents
            .Where(r => r.ReporterId == userId)
            .ToListAsync();
        foreach (var report in reports)
            report.ReporterId = null;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
