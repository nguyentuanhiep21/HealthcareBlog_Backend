using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class SavedPostService : ISavedPostService
    {
        private readonly ApplicationDbContext _context;

        public SavedPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SavePostAsync(string userId, int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var existingSave = await _context.SavedPosts
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PostId == postId);

            if (existingSave != null)
            {
                throw new BadRequestException("Post already saved.");
            }

            var savedPost = new SavedPost
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            _context.SavedPosts.Add(savedPost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnsavePostAsync(string userId, int postId)
        {
            var savedPost = await _context.SavedPosts
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PostId == postId);

            if (savedPost == null)
            {
                throw new NotFoundException("Saved post not found.");
            }

            _context.SavedPosts.Remove(savedPost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ViewPostDTO>> GetSavedPostsAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var savedPosts = await _context.SavedPosts
                .Where(sp => sp.UserId == userId)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.Likes)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.SavedByUsers)
                .OrderByDescending(sp => sp.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => sp.Post)
                .ToListAsync();

            var postDTOs = savedPosts.Select(p => p.ToViewPostDTO(userId)).ToList();
            return postDTOs;
        }

        public async Task<bool> IsPostSavedAsync(string userId, int postId)
        {
            return await _context.SavedPosts
                .AnyAsync(sp => sp.UserId == userId && sp.PostId == postId);
        }
    }
}
