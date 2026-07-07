using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

/// <summary>
/// EF Core implementation của IFollowRepository.
/// </summary>
public class FollowRepository : IFollowRepository
{
    private readonly ApplicationDbContext _context;

    public FollowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(string userId)
        => await _context.Users.FindAsync(userId);

    public async Task<Follow?> GetFollowAsync(string followerId, string followingId)
        => await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    public async Task AddAsync(Follow follow)
        => await _context.Follows.AddAsync(follow);

    public void Remove(Follow follow)
        => _context.Follows.Remove(follow);

    public async Task<List<FollowUserDTO>> GetFollowingUsersAsync(string userId, string? currentUserId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        return await _context.Follows
            .Where(f => f.FollowerId == userId)
            .Include(f => f.FollowingUser)
                .ThenInclude(u => u.Followers)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FollowUserDTO
            {
                Id = f.FollowingUser.Id!,
                FullName = f.FollowingUser.FullName ?? "Unknown",
                AvatarUrl = f.FollowingUser.AvatarUrl,
                Bio = f.FollowingUser.Bio,
                FollowerCount = f.FollowingUser.FollowerCount,
                FollowingCount = f.FollowingUser.FollowingCount,
                IsFollowedByCurrentUser = currentUserId != null
                    && f.FollowingUser.Followers.Any(follower => follower.FollowerId == currentUserId)
            })
            .ToListAsync();
    }

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
