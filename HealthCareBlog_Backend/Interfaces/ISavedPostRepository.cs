using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Interfaces;

/// <summary>
/// Repository interface cho SavedPost entity.
/// </summary>
public interface ISavedPostRepository
{
    Task<Post?> GetPostByIdAsync(int postId);
    Task<SavedPost?> GetSavedPostAsync(string userId, int postId);
    Task AddAsync(SavedPost savedPost);
    void Remove(SavedPost savedPost);
    Task<List<ViewPostDTO>> GetSavedPostsDTOAsync(string userId, int page, int pageSize);
    Task<int> SaveChangesAsync();
}
