using HealthCareBlog_Backend.Models.DTOs.Search;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface cho Search — tập trung toàn bộ EF queries tìm kiếm.
/// </summary>
public interface ISearchRepository
{
    /// <summary>Lấy danh sách admin user IDs để loại khỏi kết quả tìm kiếm</summary>
    Task<List<string>> GetAdminUserIdsAsync();

    Task<List<SearchPostResultDTO>> SearchPostsAsync(string? userId, string normalizedQuery, List<string> adminUserIds, int page, int pageSize);
    Task<int> CountPostsAsync(string normalizedQuery, List<string> adminUserIds);

    Task<List<SearchUserResultDTO>> SearchUsersAsync(string? userId, string normalizedQuery, List<string> adminUserIds, int page, int pageSize);
    Task<int> CountUsersAsync(string normalizedQuery, List<string> adminUserIds);
}
