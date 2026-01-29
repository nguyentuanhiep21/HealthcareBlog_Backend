using HealthCareBlog_Backend.Models.DTOs.Posts;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface ISavedPostService
    {
        Task<bool> SavePostAsync(string userId, int postId);
        Task<bool> UnsavePostAsync(string userId, int postId);
        Task<List<ViewPostDTO>> GetSavedPostsAsync(string userId, int page = 1, int pageSize = 20);
    }
}
