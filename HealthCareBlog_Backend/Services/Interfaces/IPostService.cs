using HealthCareBlog_Backend.Models.DTOs.Posts;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDetailDTO> CreatePostAsync(string content, List<string>? imageUrls);
        Task<PostDetailDTO> UpdatePostAsync(int postId, UpdatePostDTO updatePostDTO);
        Task<bool> DeletePostAsync(int postId, string deletedByUserId, string? deletionReason, bool isDeletedByAdmin);
    }
}
