using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface cho Post entity.
/// Định nghĩa các query phức tạp riêng của Post, ngoài các CRUD từ IRepository<T>.
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    /// <summary>Lấy post kèm đầy đủ thông tin: User, Likes, Comments, SavedByUsers</summary>
    Task<Post?> GetByIdWithDetailsAsync(int postId);

    /// <summary>Lấy danh sách post phân trang, sắp xếp mới nhất</summary>
    Task<List<Post>> GetPagedAsync(int page, int pageSize);

    /// <summary>Lấy các post trending trong ngày hôm nay (top 3 theo like + comment)</summary>
    Task<List<Post>> GetTrendingTodayAsync();

    /// <summary>Lấy danh sách post của 1 user cụ thể</summary>
    Task<List<Post>> GetByUserIdAsync(string userId, int page, int pageSize);

    /// <summary>Lấy User theo string Id (Identity)</summary>
    Task<User?> GetUserByIdAsync(string userId);

    /// <summary>Kiểm tra user đã like post chưa</summary>
    Task<LikePost?> GetLikeAsync(string userId, int postId);

    /// <summary>Thêm like vào post</summary>
    Task AddLikeAsync(LikePost like);

    /// <summary>Xóa 1 like cụ thể</summary>
    void RemoveLike(LikePost like);

    /// <summary>Xóa toàn bộ likes của 1 post (dùng khi xóa post)</summary>
    Task RemoveAllLikesByPostIdAsync(int postId);
}
