using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// FollowService — business logic cho Follow module.
/// Không còn phụ thuộc ApplicationDbContext — dùng IFollowRepository.
/// </summary>
public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly INotificationService _notificationService;

    public FollowService(IFollowRepository followRepository, INotificationService notificationService)
    {
        _followRepository = followRepository;
        _notificationService = notificationService;
    }

    public async Task<bool> FollowUserAsync(string followerId, string followingId)
    {
        if (followerId == followingId)
            throw new BadRequestException("You cannot follow yourself.");

        var follower = await _followRepository.GetUserByIdAsync(followerId)
            ?? throw new NotFoundException("User not found.");
        var following = await _followRepository.GetUserByIdAsync(followingId)
            ?? throw new NotFoundException("User not found.");

        var existingFollow = await _followRepository.GetFollowAsync(followerId, followingId);
        if (existingFollow != null)
            throw new BadRequestException("You are already following this user.");

        var newFollow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow
        };

        await _followRepository.AddAsync(newFollow);
        follower.FollowingCount++;
        following.FollowerCount++;
        await _followRepository.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            followingId,
            followerId,
            NotificationType.Follow,
            "đã bắt đầu theo dõi bạn"
        );

        return true;
    }

    public async Task<bool> UnfollowUserAsync(string followerId, string followingId)
    {
        if (followerId == followingId)
            throw new BadRequestException("Invalid operation.");

        var follow = await _followRepository.GetFollowAsync(followerId, followingId)
            ?? throw new BadRequestException("You are not following this user.");

        var follower = await _followRepository.GetUserByIdAsync(followerId)
            ?? throw new NotFoundException("User not found.");
        var following = await _followRepository.GetUserByIdAsync(followingId)
            ?? throw new NotFoundException("User not found.");

        _followRepository.Remove(follow);

        if (follower.FollowingCount > 0) follower.FollowingCount--;
        if (following.FollowerCount > 0) following.FollowerCount--;

        await _followRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<FollowUserDTO>> GetFollowingUsersAsync(
        string userId, string? currentUserId, int page = 1, int pageSize = 20)
    {
        return await _followRepository.GetFollowingUsersAsync(userId, currentUserId, page, pageSize);
    }
}
