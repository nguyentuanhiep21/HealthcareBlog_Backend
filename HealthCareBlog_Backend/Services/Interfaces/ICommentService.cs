using HealthCareBlog_Backend.Models.DTOs.Comments;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDetailDTO> CreateCommentAsync(string AuthorId, CreateCommentDTO createCommentDTO);
        Task<CommentDetailDTO> UpdateCommentAsync(string AuthorId, int commentId, UpdateCommentDTO updateCommentDTO);
        Task<bool> DeleteCommentAsync(string AuthorId, int commentId);
        Task<bool> LikeCommentAsync(string UserId, int commentId);
        Task<bool> UnlikeLikeCommentAsync(string UserId, int commentId);
    }
}
