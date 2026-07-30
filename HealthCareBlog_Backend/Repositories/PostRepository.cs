using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// EF Core implementation của IPostRepository.
/// Toàn bộ EF queries liên quan Post được tập trung ở đây.
/// </summary>
public class PostRepository : BaseRepository<Post>, IPostRepository
{
    private readonly IConnectionMultiplexer _redis;

    public PostRepository(ApplicationDbContext context, IConnectionMultiplexer redis) : base(context) 
    { 
        _redis = redis;
    }

    public async Task<Post?> GetByIdWithDetailsAsync(int postId)
        => await _dbSet
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .FirstOrDefaultAsync(p => p.Id == postId);

    public async Task<List<Post>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var posts = await _dbSet
            .Include(p => p.User)
                .ThenInclude(u => u.Followers)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        // Xáo trộn thứ tự các bài viết trong trang để tạo cảm giác random
        var random = new Random();
        return posts.OrderBy(x => random.Next()).ToList();
    }

    public async Task<List<Post>> GetTrendingOldAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var trendingPostsDb = await _dbSet
            .Include(p => p.User).ThenInclude(u => u.Followers)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .Where(p => p.CreatedAt >= today && p.CreatedAt < tomorrow)
            .OrderByDescending(p => p.LikeCount + p.CommentCount)
            .Take(3)
            .AsSplitQuery()
            .ToListAsync();

        if (trendingPostsDb.Count < 3)
        {
            var needed = 3 - trendingPostsDb.Count;
            var existingIds = trendingPostsDb.Select(p => p.Id).ToList();
            
            var recentPosts = await _dbSet
                .Include(p => p.User).ThenInclude(u => u.Followers)
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .Where(p => !existingIds.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(50)
                .AsSplitQuery()
                .ToListAsync();

            var extraPosts = recentPosts
                .OrderByDescending(p => p.LikeCount + p.CommentCount)
                .Take(needed)
                .ToList();

            trendingPostsDb.AddRange(extraPosts);
        }
        
        return trendingPostsDb;
    }

    public async Task<List<Post>> GetTrendingTodayAsync()
    {
        var db = _redis.GetDatabase();
        var lastHour = DateTime.UtcNow.AddHours(-1);

        List<Post>? trendingPostsRedis = null;
        var cachedIdsJson = await db.StringGetAsync("healthcareblog:trending_posts");
        
        if (!cachedIdsJson.IsNullOrEmpty)
        {
            try
            {
                var ids = System.Text.Json.JsonSerializer.Deserialize<List<int>>(cachedIdsJson.ToString());
                if (ids != null && ids.Count > 0)
                {
                    var posts = await _dbSet
                        .Include(p => p.User)
                        .Where(p => ids.Contains(p.Id))
                        .ToListAsync();

                    trendingPostsRedis = ids.Select(id => posts.FirstOrDefault(p => p.Id == id))
                                            .Where(p => p != null)
                                            .ToList()!;
                }
            }
            catch { }
        }

        if (trendingPostsRedis != null) 
        {
            return trendingPostsRedis;
        }

        // Fallback DB 1-hour logic
        var trendingPostsDbIds = await _dbSet
            .Where(p => p.CreatedAt >= lastHour)
            .Select(p => p.Id)
            .ToListAsync();

        if (trendingPostsDbIds.Count < 30)
        {
            var needed = 30 - trendingPostsDbIds.Count;
            var recentPosts = await _dbSet
                .Where(p => !trendingPostsDbIds.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => p.Id)
                .Take(needed)
                .ToListAsync();

            trendingPostsDbIds.AddRange(recentPosts);
        }

        var top3IdsDb = await _dbSet
            .Where(p => trendingPostsDbIds.Contains(p.Id))
            .Select(p => new { p.Id, p.LikeCount, p.CommentCount })
            .ToListAsync();

        var finalDbIds = top3IdsDb
            .OrderByDescending(p => p.LikeCount + p.CommentCount)
            .Take(3)
            .Select(p => p.Id)
            .ToList();

        var trendingPostsDb = await _dbSet
            .Include(p => p.User)
            .Where(p => finalDbIds.Contains(p.Id))
            .ToListAsync();
            
        return finalDbIds.Select(id => trendingPostsDb.First(p => p.Id == id)).ToList();
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
