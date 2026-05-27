using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface cho Follow entity.
/// </summary>
public interface IFollowRepository
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<Follow?> GetFollowAsync(string followerId, string followingId);
    Task AddAsync(Follow follow);
    void Remove(Follow follow);
    Task<List<FollowUserDTO>> GetFollowingUsersAsync(string userId, string? currentUserId, int page, int pageSize);
    Task<int> SaveChangesAsync();
}
