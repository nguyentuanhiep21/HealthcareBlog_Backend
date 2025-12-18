using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class PostMapper
    {
        public static PostDetailDTO ToPostDetailDTO(this Post post)
        {
            return new PostDetailDTO
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                AuthorId = post.UserId
            };
        }
    }
}
