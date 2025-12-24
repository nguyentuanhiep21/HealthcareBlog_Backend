using HealthCareBlog_Backend.Models.DTOs.Search;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class SearchMapper
    {
        public static SearchPostResultDTO ToSearchPostResultDTO(this Post post, string? userId)
        {
            return new SearchPostResultDTO
            {
                Id = post.Id,
                AuthorId = post.UserId,
                AuthorName = post.User?.FullName ?? "Unknown",
                AuthorAvatarUrl = post.User?.AvatarUrl,
                AuthorBio = post.User?.Bio,
                AuthorFollowerCount = post.User?.FollowerCount ?? 0,
                AuthorFollowingCount = post.User?.FollowingCount ?? 0,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                UploadTime = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLikedByCurrentUser = userId != null && post.Likes.Any(like => like.UserId == userId),
                IsSavedByCurrentUser = userId != null && post.SavedByUsers.Any(sp => sp.UserId == userId)
            };
        }

        public static SearchUserResultDTO ToSearchUserResultDTO(this User user, string? currentUserId)
        {
            return new SearchUserResultDTO
            {
                Id = user.Id!,
                FullName = user.FullName ?? "Unknown",
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                FollowerCount = user.FollowerCount,
                FollowingCount = user.FollowingCount,
                IsFollowedByCurrentUser = currentUserId != null && user.Followers.Any(f => f.FollowerId == currentUserId)
            };
        }
    }
}
