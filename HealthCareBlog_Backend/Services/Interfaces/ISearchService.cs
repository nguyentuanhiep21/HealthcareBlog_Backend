using HealthCareBlog_Backend.Models.DTOs.Search;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultDTO> SearchAllAsync(string? userId, string query, int page = 1, int pageSize = 10);
        Task<List<SearchPostResultDTO>> SearchPostsAsync(string? userId, string query, int page = 1, int pageSize = 10);
        Task<List<SearchUserResultDTO>> SearchUsersAsync(string? userId, string query, int page = 1, int pageSize = 10);
    }
}
