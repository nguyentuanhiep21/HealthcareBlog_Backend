using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Interfaces;

/// <summary>
/// Repository interface cho User entity.
/// Chỉ chứa các EF Core queries thuần — các thao tác Identity (login, token, roles)
/// vẫn do UserManager xử lý trong Service layer.
/// </summary>
public interface IUserRepository
{
    // ========== READ ==========

    /// <summary>Lấy user kèm danh sách Followers</summary>
    Task<User?> GetByIdWithFollowersAsync(string userId);

    /// <summary>Lấy danh sách user có phân trang + tìm kiếm (Admin)</summary>
    Task<List<User>> GetPagedAsync(int page, int pageSize, string? searchQuery = null);

    /// <summary>Lấy top 3 user được follow nhiều nhất, loại trừ admin</summary>
    Task<List<SuggestedUserDTO>> GetSuggestedUsersAsync(string? currentUserId);

    // ========== ADMIN STATS ==========

    Task<int> CountUsersAsync();
    Task<int> CountLockedUsersAsync();
    Task<int> CountPostsAsync();
    Task<int> CountCommentsAsync();
    Task<int> CountPendingReportsAsync();
    Task<int> CountPostsTodayAsync(DateTime today);
    Task<int> CountReportsByContentTypeAsync(string contentType);
    Task<int> CountReportsByStatusAsync(string status);

    // ========== DELETE USER (cascade cleanup) ==========

    /// <summary>Lấy tất cả posts của user kèm comments và likes để xóa cascade</summary>
    Task<List<Post>> GetUserPostsWithDetailsAsync(string userId);

    /// <summary>Xóa toàn bộ likes của danh sách post</summary>
    Task RemovePostLikesByPostIdsAsync(IEnumerable<int> postIds);

    /// <summary>Xóa toàn bộ comment likes của danh sách comment</summary>
    Task RemoveCommentLikesByCommentIdsAsync(IEnumerable<int> commentIds);

    /// <summary>Xóa toàn bộ notifications liên quan đến danh sách comment</summary>
    Task RemoveNotificationsByCommentIdsAsync(IEnumerable<int> commentIds);

    /// <summary>Xóa toàn bộ comments theo danh sách post</summary>
    Task RemoveCommentsByPostIdsAsync(IEnumerable<int> postIds);

    /// <summary>Xóa toàn bộ notifications của danh sách post</summary>
    Task RemoveNotificationsByPostIdsAsync(IEnumerable<int> postIds);

    /// <summary>Xóa toàn bộ posts của user</summary>
    Task RemovePostsByUserIdAsync(string userId);

    /// <summary>Xóa toàn bộ comments do user tạo (không phải trên bài của user)</summary>
    Task<List<int>> GetCommentIdsByUserIdAsync(string userId);
    Task RemoveCommentsByUserIdAsync(string userId);

    /// <summary>Xóa likes của user trên posts và comments</summary>
    Task RemovePostLikesByUserIdAsync(string userId);
    Task RemoveCommentLikesByUserIdAsync(string userId);

    /// <summary>Xóa follows của user</summary>
    Task RemoveFollowsByUserIdAsync(string userId);

    /// <summary>Xóa saved posts của user</summary>
    Task RemoveSavedPostsByUserIdAsync(string userId);

    /// <summary>Xóa notifications nhận và gửi của user</summary>
    Task RemoveNotificationsByUserIdAsync(string userId);

    /// <summary>Xóa reference reporter trong reports (giữ lại record để audit)</summary>
    Task ClearReporterReferenceAsync(string userId);

    /// <summary>Save changes</summary>
    Task<int> SaveChangesAsync();
}
