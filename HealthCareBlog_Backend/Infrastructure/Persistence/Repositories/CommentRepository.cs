using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation của ICommentRepository.
/// Toàn bộ EF queries liên quan Comment tập trung ở đây.
/// </summary>
public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========== READ ==========

    public async Task<Comment?> GetByIdAsync(int commentId)
        => await _context.Comments.FindAsync(commentId);

    public async Task<Comment?> GetByIdWithUserAsync(int commentId)
        => await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);

    public async Task<List<Comment>> GetByPostIdAsync(int postId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Likes)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(int postId)
        => await _context.Posts.FindAsync(postId);

    public async Task<LikeComment?> GetLikeAsync(string userId, int commentId)
        => await _context.LikeComments
            .FirstOrDefaultAsync(l => l.UserId == userId && l.CommentId == commentId);

    // ========== WRITE ==========

    public async Task AddAsync(Comment comment)
        => await _context.Comments.AddAsync(comment);

    public void Update(Comment comment)
        => _context.Comments.Update(comment);

    public void Remove(Comment comment)
        => _context.Comments.Remove(comment);

    public async Task AddLikeAsync(LikeComment like)
        => await _context.LikeComments.AddAsync(like);

    public void RemoveLike(LikeComment like)
        => _context.LikeComments.Remove(like);

    public async Task RemoveAllLikesByCommentIdAsync(int commentId)
    {
        var likes = await _context.LikeComments
            .Where(l => l.CommentId == commentId)
            .ToListAsync();
        if (likes.Any())
            _context.LikeComments.RemoveRange(likes);
    }

    public async Task RemoveNotificationsByCommentIdAsync(int commentId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.CommentId == commentId)
            .ToListAsync();
        if (notifications.Any())
            _context.Notifications.RemoveRange(notifications);
    }

    // ========== SAVE ==========

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
