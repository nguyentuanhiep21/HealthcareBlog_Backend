using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// EF Core implementation của IPostRepository.
/// Toàn bộ EF queries liên quan Post được tập trung ở đây.
/// </summary>
public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Post?> GetByIdWithDetailsAsync(int postId)
        => await _dbSet
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
            .FirstOrDefaultAsync(p => p.Id == postId);

    public async Task<List<Post>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _dbSet
            .Include(p => p.User)
                .ThenInclude(u => u.Followers)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Post>> GetTrendingTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var trendingPosts = await _dbSet
            .Include(p => p.User)
                .ThenInclude(u => u.Followers)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .Where(p => p.CreatedAt >= today && p.CreatedAt < tomorrow)
            .OrderByDescending(p => p.LikeCount + p.CommentCount)
            .Take(3)
            .ToListAsync();

        if (trendingPosts.Count < 3)
        {
            var needed = 3 - trendingPosts.Count;
            var existingIds = trendingPosts.Select(p => p.Id).ToList();

            var extraPosts = await _dbSet
                .Include(p => p.User)
                    .ThenInclude(u => u.Followers)
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .Where(p => !existingIds.Contains(p.Id))
                .OrderByDescending(p => p.LikeCount + p.CommentCount)
                .ThenByDescending(p => p.CreatedAt)
                .Take(needed)
                .ToListAsync();

            trendingPosts.AddRange(extraPosts);
        }

        return trendingPosts;
    }

    public async Task<List<Post>> GetByUserIdAsync(string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
        => await _context.Users.FindAsync(userId);

    public async Task<LikePost?> GetLikeAsync(string userId, int postId)
        => await _context.LikePosts
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

    public async Task AddLikeAsync(LikePost like)
        => await _context.LikePosts.AddAsync(like);

    public void RemoveLike(LikePost like)
        => _context.LikePosts.Remove(like);

    public async Task RemoveAllLikesByPostIdAsync(int postId)
    {
        var likes = await _context.LikePosts
            .Where(l => l.PostId == postId)
            .ToListAsync();

        if (likes.Any())
            _context.LikePosts.RemoveRange(likes);
    }

    public async Task RemoveRelatedDataByPostIdAsync(int postId)
    {
        var commentIds = await _context.Comments
            .Where(c => c.PostId == postId)
            .Select(c => c.Id)
            .ToListAsync();

        if (commentIds.Any())
        {
            var likeComments = await _context.LikeComments.Where(lc => commentIds.Contains(lc.CommentId)).ToListAsync();
            if (likeComments.Any()) _context.LikeComments.RemoveRange(likeComments);

            var commentNotifications = await _context.Notifications.Where(n => n.CommentId != null && commentIds.Contains(n.CommentId.Value)).ToListAsync();
            if (commentNotifications.Any()) _context.Notifications.RemoveRange(commentNotifications);

            var comments = await _context.Comments.Where(c => commentIds.Contains(c.Id)).ToListAsync();
            if (comments.Any()) _context.Comments.RemoveRange(comments);
        }

        var savedPosts = await _context.SavedPosts.Where(sp => sp.PostId == postId).ToListAsync();
        if (savedPosts.Any()) _context.SavedPosts.RemoveRange(savedPosts);

        var postNotifications = await _context.Notifications.Where(n => n.PostId == postId).ToListAsync();
        if (postNotifications.Any()) _context.Notifications.RemoveRange(postNotifications);
    }
}
