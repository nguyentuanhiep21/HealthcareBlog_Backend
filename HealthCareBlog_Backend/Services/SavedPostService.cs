using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// SavedPostService — business logic cho SavedPost module.
/// Không còn phụ thuộc ApplicationDbContext — dùng ISavedPostRepository.
/// </summary>
public class SavedPostService : ISavedPostService
{
    private readonly ISavedPostRepository _savedPostRepository;

    public SavedPostService(ISavedPostRepository savedPostRepository)
    {
        _savedPostRepository = savedPostRepository;
    }

    public async Task<bool> SavePostAsync(string userId, int postId)
    {
        var post = await _savedPostRepository.GetPostByIdAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        var existingSave = await _savedPostRepository.GetSavedPostAsync(userId, postId);
        if (existingSave != null)
            throw new BadRequestException("Post already saved.");

        var savedPost = new SavedPost
        {
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow
        };

        await _savedPostRepository.AddAsync(savedPost);
        await _savedPostRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnsavePostAsync(string userId, int postId)
    {
        var savedPost = await _savedPostRepository.GetSavedPostAsync(userId, postId)
            ?? throw new NotFoundException("Saved post not found.");

        _savedPostRepository.Remove(savedPost);
        await _savedPostRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<ViewPostDTO>> GetSavedPostsAsync(string userId, int page = 1, int pageSize = 20)
    {
        return await _savedPostRepository.GetSavedPostsDTOAsync(userId, page, pageSize);
    }
}
