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
    }
}
