using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class PostMapper
    {
        public static PostDetailDTO ToPostDetailDTO(this Post post, string? userId = null)
        {
            return new PostDetailDTO
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                AuthorId = post.UserId,
                IsLikedByCurrentUser = userId != null && post.Likes.Any(like => like.UserId == userId),
                IsSavedByCurrentUser = userId != null && post.SavedByUsers.Any(sp => sp.UserId == userId)
            };
        }

        public static ViewPostDTO ToViewPostDTO(this Post post, string? userId)
        {
            return new ViewPostDTO
            {
                AuthorId = post.UserId,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                UploadTime = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLikedByCurrentUser = userId != null && post.Likes.Any(like => like.UserId == userId),
                IsSavedByCurrentUser = userId != null && post.SavedByUsers.Any(sp => sp.UserId == userId)
            };
        }
    }
}
