using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class CommentMapper
    {
        public static CommentDetailDTO ToCommentDetailDTO(this Comment comment)
        {
            return new CommentDetailDTO
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new CommentUserDTO
                {
                    Id = comment.User?.Id ?? "",
                    FullName = comment.User?.FullName ?? "Unknown",
                    AvatarUrl = comment.User?.AvatarUrl,
                }
            };
        }

        public static ViewCommentDTO ToViewCommentDTO(this Comment comment, string? UserId)
        {
            return new ViewCommentDTO
            {
                Id = comment.Id,
                AuthorId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content,
                LikeCount = comment.LikeCount,
                UploadTime = comment.CreatedAt,
                IsLikedByCurrentUser = UserId != null && comment.Likes.Any(like => like.UserId == UserId),
                Author = new CommentAuthorDTO
                {
                    Id = comment.User?.Id ?? "",
                    FullName = comment.User?.FullName ?? "Unknown",
                    AvatarUrl = comment.User?.AvatarUrl,
                }
            };
        }
    }
}
