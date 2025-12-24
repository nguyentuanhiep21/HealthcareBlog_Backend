using HealthCareBlog_Backend.Models.DTOs.Comments;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<ViewCommentDTO>> ViewCommentsAsync(string? UserId, int postId, int page = 1, int pageSize = 10);  
        Task<CommentDetailDTO> CreateCommentAsync(string AuthorId, CreateCommentDTO createCommentDTO);
        Task<CommentDetailDTO> UpdateCommentAsync(string AuthorId, int commentId, UpdateCommentDTO updateCommentDTO);
        Task<bool> DeleteCommentAsync(string AuthorId, int commentId);
        Task<bool> LikeCommentAsync(string UserId, int commentId);
        Task<bool> UnlikeLikeCommentAsync(string UserId, int commentId);
        
        // Admin method
        Task<bool> AdminDeleteCommentAsync(int commentId);
    }
}
