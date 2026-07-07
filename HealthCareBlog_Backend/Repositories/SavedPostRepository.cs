using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// EF Core implementation của ISavedPostRepository.
/// </summary>
public class SavedPostRepository : ISavedPostRepository
{
    private readonly ApplicationDbContext _context;

    public SavedPostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetPostByIdAsync(int postId)
        => await _context.Posts.FindAsync(postId);

    public async Task<SavedPost?> GetSavedPostAsync(string userId, int postId)
        => await _context.SavedPosts
            .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PostId == postId);

    public async Task AddAsync(SavedPost savedPost)
        => await _context.SavedPosts.AddAsync(savedPost);

    public void Remove(SavedPost savedPost)
        => _context.SavedPosts.Remove(savedPost);

    public async Task<List<ViewPostDTO>> GetSavedPostsDTOAsync(string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var savedPosts = await _context.SavedPosts
            .Where(sp => sp.UserId == userId)
            .Include(sp => sp.Post)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Followers)
            .Include(sp => sp.Post)
                .ThenInclude(p => p.Likes)
            .Include(sp => sp.Post)
                .ThenInclude(p => p.SavedByUsers)
            .OrderByDescending(sp => sp.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return savedPosts.Select(sp => sp.Post.ToViewPostDTO(userId)).ToList();
    }

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
