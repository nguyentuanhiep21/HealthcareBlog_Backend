using HealthCareBlog_Backend.Models.DTOs.Posts;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDetailDTO> CreatePostAsync(string AuthorId, CreatePostDTO createPostDTO);
        Task<PostDetailDTO> UpdatePostAsync(int postId, UpdatePostDTO updatePostDTO);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> LikePostAsync(string UserId, int postId);
        Task<bool> UnlikePostAsync(string UserId, int postId);
    }
}
