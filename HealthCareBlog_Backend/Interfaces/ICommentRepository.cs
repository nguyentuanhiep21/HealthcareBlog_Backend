using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Interfaces;

/// <summary>
/// Repository interface cho Comment entity.
/// Tập trung toàn bộ EF queries liên quan Comment và LikeComment.
/// </summary>
public interface ICommentRepository
{
    // ========== READ ==========

    /// <summary>Lấy comment theo Id</summary>
    Task<Comment?> GetByIdAsync(int commentId);

    /// <summary>Lấy comment kèm User data (sau khi tạo/cập nhật)</summary>
    Task<Comment?> GetByIdWithUserAsync(int commentId);

    /// <summary>Lấy danh sách comments gốc của 1 post, phân trang</summary>
    Task<List<Comment>> GetByPostIdAsync(int postId, int page, int pageSize);

    /// <summary>Lấy danh sách trả lời của 1 comment, phân trang</summary>
    Task<List<Comment>> GetRepliesByCommentIdAsync(int parentCommentId, int page, int pageSize);

    /// <summary>Lấy post để cập nhật CommentCount</summary>
    Task<Post?> GetPostByIdAsync(int postId);

    /// <summary>Kiểm tra user đã like comment chưa</summary>
    Task<LikeComment?> GetLikeAsync(string userId, int commentId);

    // ========== WRITE ==========

    Task AddAsync(Comment comment);
    void Update(Comment comment);
    void Remove(Comment comment);

    Task AddLikeAsync(LikeComment like);
    void RemoveLike(LikeComment like);

    /// <summary>Xóa toàn bộ likes của 1 comment (dùng khi xóa comment)</summary>
    Task RemoveAllLikesByCommentIdAsync(int commentId);

    /// <summary>Xóa toàn bộ notifications liên quan đến comment</summary>
    Task RemoveNotificationsByCommentIdAsync(int commentId);

    // ========== SAVE ==========

    Task<int> SaveChangesAsync();
}
