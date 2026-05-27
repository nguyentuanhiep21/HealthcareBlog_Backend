using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========== READ ==========

    public async Task<ReportedContent?> GetByIdAsync(int reportId)
        => await _context.ReportedContents.FindAsync(reportId);

    public async Task<ReportedContent?> GetExistingPendingAsync(string reporterId, string contentType, string contentId)
        => await _context.ReportedContents
            .FirstOrDefaultAsync(r => r.ReporterId == reporterId
                && r.ContentType == contentType
                && r.ContentId == contentId
                && r.Status == "Pending");

    public async Task<List<ReportedContent>> GetPagedAsync(int page, int pageSize, string? status, string? contentType)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.ReportedContents.AsQueryable();
        if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);
        if (!string.IsNullOrEmpty(contentType)) query = query.Where(r => r.ContentType == contentType);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // ========== CONTENT LOOKUPS ==========

    public async Task<User?> GetUserByIdAsync(string userId)
        => await _context.Users.FindAsync(userId);

    public async Task<(User? author, string? preview)> GetPostTargetInfoAsync(int postId)
    {
        var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);
        return (post?.User, post?.Content);
    }

    public async Task<(User? author, string? preview)> GetCommentTargetInfoAsync(int commentId)
    {
        var comment = await _context.Comments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null) return (null, null);
        var preview = comment.Content.Length > 100 ? comment.Content[..100] + "..." : comment.Content;
        return (comment.User, preview);
    }

    // ========== EXISTENCE CHECKS ==========

    public Task<bool> PostExistsAsync(string postId)
        => int.TryParse(postId, out var id)
            ? _context.Posts.AnyAsync(p => p.Id == id)
            : Task.FromResult(false);

    public Task<bool> CommentExistsAsync(string commentId)
        => int.TryParse(commentId, out var id)
            ? _context.Comments.AnyAsync(c => c.Id == id)
            : Task.FromResult(false);

    public Task<bool> UserExistsAsync(string userId)
        => _context.Users.AnyAsync(u => u.Id == userId);

    // ========== WRITE ==========

    public async Task AddAsync(ReportedContent report)
        => await _context.ReportedContents.AddAsync(report);

    // ========== CASCADE DELETE HELPERS ==========

    public async Task<List<Post>> GetPostsByUserIdAsync(string userId)
        => await _context.Posts.Where(p => p.UserId == userId).ToListAsync();

    public async Task<List<int>> GetCommentIdsByPostIdsAsync(IEnumerable<int> postIds)
        => await _context.Comments.Where(c => postIds.Contains(c.PostId)).Select(c => c.Id).ToListAsync();

    public async Task<List<int>> GetCommentIdsByUserIdAsync(string userId)
        => await _context.Comments.Where(c => c.UserId == userId).Select(c => c.Id).ToListAsync();

    public async Task RemoveCommentLikesByCommentIdsAsync(IEnumerable<int> commentIds)
    {
        var likes = await _context.LikeComments.Where(l => commentIds.Contains(l.CommentId)).ToListAsync();
        if (likes.Any()) _context.LikeComments.RemoveRange(likes);
    }

    public async Task RemoveCommentsByPostIdsAsync(IEnumerable<int> postIds)
    {
        var comments = await _context.Comments.Where(c => postIds.Contains(c.PostId)).ToListAsync();
        if (comments.Any()) _context.Comments.RemoveRange(comments);
    }

    public async Task RemovePostLikesByPostIdsAsync(IEnumerable<int> postIds)
    {
        var likes = await _context.LikePosts.Where(l => postIds.Contains(l.PostId)).ToListAsync();
        if (likes.Any()) _context.LikePosts.RemoveRange(likes);
    }

    public async Task RemoveSavedPostsByPostIdsAsync(IEnumerable<int> postIds)
    {
        var saved = await _context.SavedPosts.Where(sp => postIds.Contains(sp.PostId)).ToListAsync();
        if (saved.Any()) _context.SavedPosts.RemoveRange(saved);
    }

    public async Task RemoveNotificationsByPostIdsAsync(IEnumerable<int> postIds)
    {
        var n = await _context.Notifications.Where(x => x.PostId.HasValue && postIds.Contains(x.PostId.Value)).ToListAsync();
        if (n.Any()) _context.Notifications.RemoveRange(n);
    }

    public async Task RemoveNotificationsByCommentIdsAsync(IEnumerable<int> commentIds)
    {
        var n = await _context.Notifications.Where(x => x.CommentId.HasValue && commentIds.Contains(x.CommentId.Value)).ToListAsync();
        if (n.Any()) _context.Notifications.RemoveRange(n);
    }

    public async Task RemovePostsByUserIdAsync(string userId)
    {
        var posts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
        if (posts.Any()) _context.Posts.RemoveRange(posts);
    }

    public async Task RemoveCommentLikesByUserIdAsync(string userId)
    {
        var likes = await _context.LikeComments.Where(l => l.UserId == userId).ToListAsync();
        if (likes.Any()) _context.LikeComments.RemoveRange(likes);
    }

    public async Task RemoveCommentsByUserIdAsync(IEnumerable<int> commentIds)
    {
        var comments = await _context.Comments.Where(c => commentIds.Contains(c.Id)).ToListAsync();
        if (comments.Any()) _context.Comments.RemoveRange(comments);
    }

    public async Task RemovePostLikesByUserIdAsync(string userId)
    {
        var likes = await _context.LikePosts.Where(l => l.UserId == userId).ToListAsync();
        if (likes.Any()) _context.LikePosts.RemoveRange(likes);
    }

    public async Task RemoveFollowsByUserIdAsync(string userId)
    {
        var follows = await _context.Follows.Where(f => f.FollowerId == userId || f.FollowingId == userId).ToListAsync();
        if (follows.Any()) _context.Follows.RemoveRange(follows);
    }

    public async Task RemoveSavedPostsByUserIdAsync(string userId)
    {
        var saved = await _context.SavedPosts.Where(sp => sp.UserId == userId).ToListAsync();
        if (saved.Any()) _context.SavedPosts.RemoveRange(saved);
    }

    public async Task RemoveNotificationsByUserIdAsync(string userId, IEnumerable<int> postIds, IEnumerable<int> commentIds)
    {
        var postIdList = postIds.ToList();
        var commentIdList = commentIds.ToList();
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId || n.ActorId == userId
                || (n.PostId.HasValue && postIdList.Contains(n.PostId.Value))
                || (n.CommentId.HasValue && commentIdList.Contains(n.CommentId.Value)))
            .ToListAsync();
        if (notifications.Any()) _context.Notifications.RemoveRange(notifications);
    }

    public async Task RemoveReportsByUserIdExceptCurrentAsync(string userId, int currentReportId)
    {
        var reports = await _context.ReportedContents
            .Where(r => r.ReporterId == userId && r.Id != currentReportId)
            .ToListAsync();
        if (reports.Any()) _context.ReportedContents.RemoveRange(reports);
    }

    // ========== POST REPORT cascade ==========

    public async Task<Post?> GetPostByIdAsync(int postId)
        => await _context.Posts.FindAsync(postId);

    public Task RemoveCommentsAsync(IEnumerable<Comment> comments)
    {
        var list = comments.ToList();
        if (list.Any()) _context.Comments.RemoveRange(list);
        return Task.CompletedTask;
    }

    public void _removePost(Post post) => _context.Posts.Remove(post);
    public Task RemovePostAsync(Post post) { _removePost(post); return Task.CompletedTask; }

    public async Task UpdatePostCommentCountAsync(Dictionary<int, int> postIdToDecrement)
    {
        foreach (var (postId, count) in postIdToDecrement)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
                post.CommentCount = Math.Max(0, post.CommentCount - count);
        }
    }

    // ========== COMMENT REPORT cascade ==========

    public async Task<Comment?> GetCommentByIdAsync(int commentId)
        => await _context.Comments.FindAsync(commentId);

    public async Task<Post?> GetPostForCommentAsync(int postId)
        => await _context.Posts.FindAsync(postId);

    public Task RemoveCommentAsync(Comment comment) { _context.Comments.Remove(comment); return Task.CompletedTask; }

    // ========== SAVE ==========

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
