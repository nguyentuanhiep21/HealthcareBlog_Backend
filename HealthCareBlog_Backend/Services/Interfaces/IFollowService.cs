using HealthCareBlog_Backend.Models.DTOs.Users;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IFollowService
    {
        Task<bool> FollowUserAsync(string followerId, string followingId);
        Task<bool> UnfollowUserAsync(string followerId, string followingId);
        Task<List<string>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<string>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<FollowUserDTO>> GetFollowingUsersAsync(string userId, string? currentUserId, int page = 1, int pageSize = 20);
        Task<List<FollowUserDTO>> GetFollowersUsersAsync(string userId, string? currentUserId, int page = 1, int pageSize = 20);
        Task<bool> IsFollowingAsync(string followerId, string followingId);
    }
}
