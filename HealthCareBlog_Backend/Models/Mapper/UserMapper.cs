using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class UserMapper
    {
        public static ViewAccountDTO ToViewAccountDTO(this User user)
        {
            return new ViewAccountDTO
            {
                Id = user.Id!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                AvatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? "/images/logo.png" : user.AvatarUrl
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
                AvatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? "/images/logo.png" : user.AvatarUrl,
                FollowerCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
                PostCount = user.PostCount,
                IsFollowedByCurrentUser = currentUserId != null && user.Followers.Any(f => f.FollowerId == currentUserId)
            };
        }
    }
}
