using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Search;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SearchResultDTO> SearchAllAsync(string? userId, string query, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            if (string.IsNullOrWhiteSpace(query))
            {
                return new SearchResultDTO();
            }

            var normalizedQuery = query.ToLower().Trim();

            // Get all admin user IDs
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var adminUserIds = adminRole != null 
                ? await _context.UserRoles
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync()
                : new List<string>();

            var posts = await SearchPostsInternalAsync(userId, normalizedQuery, page, pageSize);
            var users = await SearchUsersInternalAsync(userId, normalizedQuery, page, pageSize);

            var totalPosts = await _context.Posts
                .Where(p => !adminUserIds.Contains(p.UserId) && p.Content.ToLower().Contains(normalizedQuery))
                .CountAsync();

            var totalUsers = await _context.Users
                .Where(u => !adminUserIds.Contains(u.Id) && 
                           (u.FullName!.ToLower().Contains(normalizedQuery) || 
                            u.Email!.ToLower().Contains(normalizedQuery)))
                .CountAsync();

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
            {
                return new List<SearchPostResultDTO>();
            }

            var normalizedQuery = query.ToLower().Trim();
            return await SearchPostsInternalAsync(userId, normalizedQuery, page, pageSize);
        }

        public async Task<List<SearchUserResultDTO>> SearchUsersAsync(string? userId, string query, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<SearchUserResultDTO>();
            }

            var normalizedQuery = query.ToLower().Trim();
            return await SearchUsersInternalAsync(userId, normalizedQuery, page, pageSize);
        }

        private async Task<List<SearchPostResultDTO>> SearchPostsInternalAsync(string? userId, string normalizedQuery, int page, int pageSize)
        {
            // Get all admin user IDs
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var adminUserIds = adminRole != null 
                ? await _context.UserRoles
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync()
                : new List<string>();

            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .Where(p => !adminUserIds.Contains(p.UserId) && p.Content.ToLower().Contains(normalizedQuery))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return posts.Select(p => p.ToSearchPostResultDTO(userId)).ToList();
        }

        private async Task<List<SearchUserResultDTO>> SearchUsersInternalAsync(string? userId, string normalizedQuery, int page, int pageSize)
        {
            // Get all admin user IDs
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var adminUserIds = adminRole != null 
                ? await _context.UserRoles
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync()
                : new List<string>();

            var users = await _context.Users
                .Include(u => u.Followers)
                .Where(u => !adminUserIds.Contains(u.Id) && 
                           (u.FullName!.ToLower().Contains(normalizedQuery) || 
                            u.Email!.ToLower().Contains(normalizedQuery)))
                .OrderByDescending(u => u.FollowerCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(u => u.ToSearchUserResultDTO(userId)).ToList();
        }
    }
}
