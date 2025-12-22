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
                CommentId = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public static ViewCommentDTO ToViewCommentDTO(this Comment comment, string? UserId)
        {
            return new ViewCommentDTO
            {
                AuthorId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content,
                LikeCount = comment.LikeCount,
                UploadTime = comment.CreatedAt,
                IsLikedByCurrentUser = UserId != null && comment.Likes.Any(like => like.UserId == UserId)
            };
        }
    }
}
