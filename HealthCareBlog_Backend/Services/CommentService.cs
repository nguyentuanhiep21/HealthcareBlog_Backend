using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class CommentService : ICommentService
    {
        public readonly ApplicationDbContext _context;
        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentDetailDTO> CreateCommentAsync(string AuthorId, CreateCommentDTO createCommentDTO)
        {
            if (createCommentDTO == null)
            {
                throw new BadRequestException("Invalid data.");
            }

            var newComment = new Comment
            {
                UserId = AuthorId,
                PostId = createCommentDTO.PostId,
                Content = createCommentDTO.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return newComment.ToCommentDetailDTO();
        }

        public async Task<CommentDetailDTO> UpdateCommentAsync(string AuthorId, int commentId, UpdateCommentDTO updateCommentDTO)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            if (comment.UserId != AuthorId)
            {
                throw new UnauthorizedException("You are not authorized to update this comment.");
            }

            comment.Content = updateCommentDTO.Content;
            await _context.SaveChangesAsync();
            return comment.ToCommentDetailDTO();
        }

        public async Task<bool> DeleteCommentAsync(string AuthorId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            if (comment.UserId != AuthorId)
            {
                throw new UnauthorizedException("You are not authorized to delete this comment.");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeCommentAsync(string UserId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.CommentId == commentId);

            if (existingLike != null)
            {
                throw new BadRequestException("You have already liked this comment.");
            }

            var newLike = new Like
            {
                UserId = UserId,
                CommentId = commentId,
                PostId = null, 
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(newLike);
            comment.LikeCount++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikeLikeCommentAsync(string UserId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.CommentId == commentId);

            if (existingLike == null)
            {
                throw new BadRequestException("You have not liked this comment yet.");
            }

            _context.Likes.Remove(existingLike);

            if (comment.LikeCount > 0)
            {
                comment.LikeCount--;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
