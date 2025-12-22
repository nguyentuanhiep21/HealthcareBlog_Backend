using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class FollowService : IFollowService
    {
        public readonly ApplicationDbContext _context;

        public FollowService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FollowUserAsync(string followerId, string followingId)
        {
            if (followerId == followingId)
            {
                throw new BadRequestException("You cannot follow yourself.");
            }

            var follower = await _context.Users.FindAsync(followerId);
            var following = await _context.Users.FindAsync(followingId);

            if (follower == null || following == null)
            {
                throw new NotFoundException("User not found.");
            }

            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (existingFollow != null)
            {
                throw new BadRequestException("You are already following this user.");
            }

            var newFollow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Follows.Add(newFollow);

            follower.FollowingCount++;  
            following.FollowerCount++;  

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowUserAsync(string followerId, string followingId)
        {
            if (followerId == followingId)
            {
                throw new BadRequestException("Invalid operation.");
            }

            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null)
            {
                throw new BadRequestException("You are not following this user.");
            }

            var follower = await _context.Users.FindAsync(followerId);
            var following = await _context.Users.FindAsync(followingId);

            if (follower == null || following == null)
            {
                throw new NotFoundException("User not found.");
            }

            _context.Follows.Remove(follow);

            if (follower.FollowingCount > 0)
                follower.FollowingCount--;

            if (following.FollowerCount > 0)
                following.FollowerCount--;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var followerIds = await _context.Follows
                .Where(f => f.FollowingId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => f.FollowerId)
                .ToListAsync();

            return followerIds;
        }

        public async Task<List<string>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => f.FollowingId)
                .ToListAsync();

            return followingIds;
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followingId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }
    }
}
