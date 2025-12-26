using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class CommentService : ICommentService
    {
        public readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public CommentService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<ViewCommentDTO>> ViewCommentsAsync(string? UserId, int postId, int page = 1, int pageSize = 10)
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa 100 comments mỗi lần
            var comments = await _context.Comments
                .Include(c => c.User)  
                .Include(c => c.Likes)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize) // Bỏ qua các comments của các trang trước
                .Take(pageSize) // Lấy số lượng comments theo pageSize
                .ToListAsync();
            var commentDTOs = comments.Select(c => c.ToViewCommentDTO(UserId)).ToList();
            return commentDTOs;
        }

        public async Task<CommentDetailDTO> CreateCommentAsync(string AuthorId, CreateCommentDTO createCommentDTO)
        {
            if (createCommentDTO == null)
            {
                throw new BadRequestException("Invalid data.");
            }

            // Kiểm tra post tồn tại
            var post = await _context.Posts.FindAsync(createCommentDTO.PostId);
            if (post == null)
            {
                throw new NotFoundException("Không tìm thấy bài viết.");
            }

            var newComment = new Comment
            {
                UserId = AuthorId,
                PostId = createCommentDTO.PostId,
                Content = createCommentDTO.Content,
                CreatedAt = DateTimeHelper.GetVietnamTime()
            };

            _context.Comments.Add(newComment);
            
            // Tăng comment count của post
            post.CommentCount++;
            
            await _context.SaveChangesAsync();
            
            // Create notification for post owner
            await _notificationService.CreateNotificationAsync(
                post.UserId, 
                AuthorId, 
                NotificationType.Comment, 
                "đã bình luận về bài viết của bạn",
                createCommentDTO.PostId,
                newComment.Id
            );
            
            // Reload comment with User data
            var commentWithUser = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == newComment.Id);
            
            return commentWithUser!.ToCommentDetailDTO();
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
            
            // Reload comment with User data
            var commentWithUser = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            
            return commentWithUser!.ToCommentDetailDTO();
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

            var commentLikes = await _context.LikeComments
                .Where(l => l.CommentId == commentId)
                .ToListAsync();
            
            if (commentLikes.Any())
            {
                _context.LikeComments.RemoveRange(commentLikes);
            }

            var notifications = await _context.Notifications
                .Where(n => n.CommentId == commentId)
                .ToListAsync();
            
            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
            }

            // Decrement post comment count
            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post != null && post.CommentCount > 0)
            {
                post.CommentCount--;
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

            var existingLike = await _context.LikeComments
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.CommentId == commentId);

            if (existingLike != null)
            {
                // Đã like rồi, unlike luôn
                _context.LikeComments.Remove(existingLike);
                if (comment.LikeCount > 0)
                {
                    comment.LikeCount--;
                }
                await _context.SaveChangesAsync();
                return false; // Return false để biết là đã unlike
            }

            var newLike = new LikeComment
            {
                UserId = UserId,
                CommentId = commentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.LikeComments.Add(newLike);
            comment.LikeCount++;
            await _context.SaveChangesAsync();
            return true; // Return true để biết là đã like
        }

        public async Task<bool> UnlikeLikeCommentAsync(string UserId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            var existingLike = await _context.LikeComments
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.CommentId == commentId);

            if (existingLike == null)
            {
                // Chưa like, like luôn
                var newLike = new LikeComment
                {
                    UserId = UserId,
                    CommentId = commentId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.LikeComments.Add(newLike);
                comment.LikeCount++;
                await _context.SaveChangesAsync();
                return false; // Return false để biết là đã like
            }

            _context.LikeComments.Remove(existingLike);

            if (comment.LikeCount > 0)
            {
                comment.LikeCount--;
            }

            await _context.SaveChangesAsync();
            return true; // Return true để biết là đã unlike
        }

        // Admin method
        public async Task<bool> AdminDeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            var post = await _context.Posts.FindAsync(comment.PostId);

            var commentLikes = await _context.LikeComments
                .Where(l => l.CommentId == commentId)
                .ToListAsync();

            if (commentLikes.Any())
            {
                _context.LikeComments.RemoveRange(commentLikes);
            }

            _context.Comments.Remove(comment);

            if (post != null && post.CommentCount > 0)
            {
                post.CommentCount--;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
