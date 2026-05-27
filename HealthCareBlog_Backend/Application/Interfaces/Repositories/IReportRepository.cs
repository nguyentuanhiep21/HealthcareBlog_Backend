using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface cho Report entity.
/// ReportService vẫn dùng UserManager cho các Identity operations (lock, delete user).
/// </summary>
public interface IReportRepository
{
    // ========== READ ==========
    Task<ReportedContent?> GetByIdAsync(int reportId);
    Task<ReportedContent?> GetExistingPendingAsync(string reporterId, string contentType, string contentId);
    Task<List<ReportedContent>> GetPagedAsync(int page, int pageSize, string? status, string? contentType);

    // ========== CONTENT LOOKUPS (dùng trong GetTargetInfo) ==========
    Task<User?> GetUserByIdAsync(string userId);
    Task<(User? author, string? preview)> GetPostTargetInfoAsync(int postId);
    Task<(User? author, string? preview)> GetCommentTargetInfoAsync(int commentId);

    // ========== EXISTENCE CHECKS ==========
    Task<bool> PostExistsAsync(string postId);
    Task<bool> CommentExistsAsync(string commentId);
    Task<bool> UserExistsAsync(string userId);

    // ========== WRITE ==========
    Task AddAsync(ReportedContent report);

    // ========== CASCADE DELETE helpers (dùng trong ProcessUserReport/ProcessPostReport) ==========
    Task<List<Post>> GetPostsByUserIdAsync(string userId);
    Task<List<int>> GetCommentIdsByPostIdsAsync(IEnumerable<int> postIds);
    Task<List<int>> GetCommentIdsByUserIdAsync(string userId);
    Task RemoveCommentLikesByCommentIdsAsync(IEnumerable<int> commentIds);
    Task RemoveCommentsByPostIdsAsync(IEnumerable<int> postIds);
    Task RemovePostLikesByPostIdsAsync(IEnumerable<int> postIds);
    Task RemoveSavedPostsByPostIdsAsync(IEnumerable<int> postIds);
    Task RemoveNotificationsByPostIdsAsync(IEnumerable<int> postIds);
    Task RemoveNotificationsByCommentIdsAsync(IEnumerable<int> commentIds);
    Task RemovePostsByUserIdAsync(string userId);
    Task RemoveCommentLikesByUserIdAsync(string userId);
    Task RemoveCommentsByUserIdAsync(IEnumerable<int> commentIds);
    Task RemovePostLikesByUserIdAsync(string userId);
    Task RemoveFollowsByUserIdAsync(string userId);
    Task RemoveSavedPostsByUserIdAsync(string userId);
    Task RemoveNotificationsByUserIdAsync(string userId, IEnumerable<int> postIds, IEnumerable<int> commentIds);
    Task RemoveReportsByUserIdExceptCurrentAsync(string userId, int currentReportId);

    // ========== POST REPORT cascade ==========
    Task<Post?> GetPostByIdAsync(int postId);
    Task RemoveCommentsAsync(IEnumerable<Comment> comments);
    Task RemovePostAsync(Post post);
    Task UpdatePostCommentCountAsync(Dictionary<int, int> postIdToDecrement);

    // ========== COMMENT REPORT cascade ==========
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task<Post?> GetPostForCommentAsync(int postId);
    Task RemoveCommentAsync(Comment comment);

    // ========== SAVE ==========
    Task<int> SaveChangesAsync();
}
