using HealthCareBlog_Backend.Models.DTOs.Posts;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IPostService
    {
        Task<List<ViewPostDTO>> ViewPostAsync(string? UserId, int page = 1, int pageSize = 10);
        Task<PostDetailDTO> GetPostByIdAsync(string? UserId, int postId);
        Task<PostDetailDTO> CreatePostAsync(string AuthorId, CreatePostDTO createPostDTO);
        Task<PostDetailDTO> UpdatePostAsync(string AuthorId, int postId, UpdatePostDTO updatePostDTO);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> LikePostAsync(string UserId, int postId);
        Task<bool> UnlikePostAsync(string UserId, int postId);
        Task<List<ViewPostDTO>> GetTrendingPostsAsync(string? UserId);
        
        // Admin methods
        Task<List<AdminPostDTO>> GetAllPostsForAdminAsync(string? userId, int page = 1, int pageSize = 20, string? searchQuery = null);
    }
}
