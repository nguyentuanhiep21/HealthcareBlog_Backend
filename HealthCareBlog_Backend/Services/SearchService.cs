using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Search;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// SearchService — business logic cho Search module.
/// Không còn phụ thuộc ApplicationDbContext — dùng ISearchRepository.
/// Admin user IDs chỉ query 1 lần thay vì 3 lần như trước.
/// </summary>
public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;

    public SearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<SearchResultDTO> SearchAllAsync(string? userId, string query, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        if (string.IsNullOrWhiteSpace(query))
            return new SearchResultDTO();

        var normalizedQuery = query.ToLower().Trim();
        var adminUserIds = await _searchRepository.GetAdminUserIdsAsync();

        var posts = await _searchRepository.SearchPostsAsync(userId, normalizedQuery, adminUserIds, page, pageSize);
        var users = await _searchRepository.SearchUsersAsync(userId, normalizedQuery, adminUserIds, page, pageSize);
        var totalPosts = await _searchRepository.CountPostsAsync(normalizedQuery, adminUserIds);
        var totalUsers = await _searchRepository.CountUsersAsync(normalizedQuery, adminUserIds);

        return new SearchResultDTO
        {
            Posts = posts,
            Users = users,
            TotalPosts = totalPosts,
            TotalUsers = totalUsers
        };
    }

    public async Task<List<SearchPostResultDTO>> SearchPostsAsync(string? userId, string query, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        if (string.IsNullOrWhiteSpace(query))
            return new List<SearchPostResultDTO>();

        var normalizedQuery = query.ToLower().Trim();
        var adminUserIds = await _searchRepository.GetAdminUserIdsAsync();
        return await _searchRepository.SearchPostsAsync(userId, normalizedQuery, adminUserIds, page, pageSize);
    }

    public async Task<List<SearchUserResultDTO>> SearchUsersAsync(string? userId, string query, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        if (string.IsNullOrWhiteSpace(query))
            return new List<SearchUserResultDTO>();

        var normalizedQuery = query.ToLower().Trim();
        var adminUserIds = await _searchRepository.GetAdminUserIdsAsync();
        return await _searchRepository.SearchUsersAsync(userId, normalizedQuery, adminUserIds, page, pageSize);
    }
}
