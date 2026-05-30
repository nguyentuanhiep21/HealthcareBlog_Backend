using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class UserMapper
    {
        public static async Task<ViewAccountDTO> ToViewAccountDTOAsync(this User user, UserManager<User> userManager)
        {
            var roles = await userManager.GetRolesAsync(user);
            return new ViewAccountDTO
            {
                Id = user.Id!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                FollowerCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
                IsAdmin = roles.Contains("Admin")
            };
        }

        public static UserProfileDTO ToUserProfileDTO(this User user, string? currentUserId)
        {
            return new UserProfileDTO
            {
                Id = user.Id!,
                FullName = user.FullName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                FollowerCount = user.Followers?.Count ?? 0,
                FollowingCount = user.FollowingCount,
                PostCount = user.PostCount,
                IsFollowedByCurrentUser = currentUserId != null && user.Followers.Any(f => f.FollowerId == currentUserId)
            };
        }
    }
}
