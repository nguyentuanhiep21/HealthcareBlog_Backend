using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Search;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly ApplicationDbContext _context;

    public SearchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetAdminUserIdsAsync()
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null) return new List<string>();

        return await _context.UserRoles
            .Where(ur => ur.RoleId == adminRole.Id)
            .Select(ur => ur.UserId)
            .ToListAsync();
    }

    public async Task<List<SearchPostResultDTO>> SearchPostsAsync(
        string? userId, string normalizedQuery, List<string> adminUserIds, int page, int pageSize)
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.SavedByUsers)
            .Where(p => !adminUserIds.Contains(p.UserId)
                && p.Content.ToLower().Contains(normalizedQuery))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return posts.Select(p => p.ToSearchPostResultDTO(userId)).ToList();
    }

    public Task<int> CountPostsAsync(string normalizedQuery, List<string> adminUserIds)
        => _context.Posts
            .Where(p => !adminUserIds.Contains(p.UserId)
                && p.Content.ToLower().Contains(normalizedQuery))
            .CountAsync();

    public async Task<List<SearchUserResultDTO>> SearchUsersAsync(
        string? userId, string normalizedQuery, List<string> adminUserIds, int page, int pageSize)
    {
        var users = await _context.Users
            .Include(u => u.Followers)
            .Where(u => !adminUserIds.Contains(u.Id)
                && (u.FullName!.ToLower().Contains(normalizedQuery)
                    || u.Email!.ToLower().Contains(normalizedQuery)))
            .OrderByDescending(u => u.FollowerCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return users.Select(u => u.ToSearchUserResultDTO(userId)).ToList();
    }

    public Task<int> CountUsersAsync(string normalizedQuery, List<string> adminUserIds)
        => _context.Users
            .Where(u => !adminUserIds.Contains(u.Id)
                && (u.FullName!.ToLower().Contains(normalizedQuery)
                    || u.Email!.ToLower().Contains(normalizedQuery)))
            .CountAsync();
}
