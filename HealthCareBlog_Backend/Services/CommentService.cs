using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Helpers;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// CommentService — business logic cho Comment module.
/// Không còn phụ thuộc ApplicationDbContext — dùng ICommentRepository.
/// </summary>
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly INotificationService _notificationService;

    public CommentService(ICommentRepository commentRepository, INotificationService notificationService)
    {
        _commentRepository = commentRepository;
        _notificationService = notificationService;
    }

    public async Task<List<ViewCommentDTO>> ViewCommentsAsync(string? userId, int postId, int page = 1, int pageSize = 10)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId, page, pageSize);
        return comments.Select(c => c.ToViewCommentDTO(userId)).ToList();
    }

    public async Task<List<ViewCommentDTO>> ViewRepliesAsync(string? userId, int commentId, int page = 1, int pageSize = 10)
    {
        var replies = await _commentRepository.GetRepliesByCommentIdAsync(commentId, page, pageSize);
        return replies.Select(c => c.ToViewCommentDTO(userId)).ToList();
    }

    public async Task<CommentDetailDTO> CreateCommentAsync(string authorId, CreateCommentDTO createCommentDTO)
    {
        if (createCommentDTO == null)
            throw new BadRequestException("Invalid data.");

        var post = await _commentRepository.GetPostByIdAsync(createCommentDTO.PostId)
            ?? throw new NotFoundException("Không tìm thấy bài viết.");

        Comment? parentComment = null;
        if (createCommentDTO.ParentCommentId.HasValue)
        {
            parentComment = await _commentRepository.GetByIdAsync(createCommentDTO.ParentCommentId.Value)
                ?? throw new NotFoundException("Không tìm thấy bình luận cha.");
            if (parentComment.PostId != createCommentDTO.PostId)
                throw new BadRequestException("Bình luận cha không thuộc bài viết này.");
            if (parentComment.ParentCommentId != null)
                throw new BadRequestException("Chỉ hỗ trợ trả lời bình luận gốc (1 cấp).");
        }

        var newComment = new Comment
        {
            UserId = authorId,
            PostId = createCommentDTO.PostId,
            ParentCommentId = createCommentDTO.ParentCommentId,
            Content = createCommentDTO.Content,
            CreatedAt = DateTimeHelper.GetVietnamTime()
        };

        await _commentRepository.AddAsync(newComment);
        if (parentComment != null) parentComment.ReplyCount++;
        post.CommentCount++;
        await _commentRepository.SaveChangesAsync();

        // Gửi notification
        if (parentComment != null)
        {
            await _notificationService.CreateNotificationAsync(
                parentComment.UserId,
                authorId,
                NotificationType.Comment,
                "đã trả lời bình luận của bạn",
                createCommentDTO.PostId,
                newComment.Id
            );
        }
        else
        {
            await _notificationService.CreateNotificationAsync(
                post.UserId,
                authorId,
                NotificationType.Comment,
                "đã bình luận về bài viết của bạn",
                createCommentDTO.PostId,
                newComment.Id
            );
        }

        // Reload với User data để map DTO
        var commentWithUser = await _commentRepository.GetByIdWithUserAsync(newComment.Id);
        return commentWithUser!.ToCommentDetailDTO();
    }

    public async Task<CommentDetailDTO> UpdateCommentAsync(string authorId, int commentId, UpdateCommentDTO updateCommentDTO)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Comment not found.");

        if (comment.UserId != authorId)
            throw new UnauthorizedException("You are not authorized to update this comment.");

        comment.Content = updateCommentDTO.Content;
        await _commentRepository.SaveChangesAsync();

        var commentWithUser = await _commentRepository.GetByIdWithUserAsync(commentId);
        return commentWithUser!.ToCommentDetailDTO();
    }

    public async Task<bool> DeleteCommentAsync(string authorId, int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Comment not found.");

        if (comment.UserId != authorId)
            throw new UnauthorizedException("You are not authorized to delete this comment.");

        await _commentRepository.RemoveAllLikesByCommentIdAsync(commentId);
        await _commentRepository.RemoveNotificationsByCommentIdAsync(commentId);

        var post = await _commentRepository.GetPostByIdAsync(comment.PostId);
        if (comment.ParentCommentId.HasValue)
        {
            var parent = await _commentRepository.GetByIdAsync(comment.ParentCommentId.Value);
            if (parent != null && parent.ReplyCount > 0) parent.ReplyCount--;
            if (post != null && post.CommentCount > 0) post.CommentCount--;
        }
        else
        {
            var decreaseAmount = 1 + comment.ReplyCount;
            if (post != null && post.CommentCount >= decreaseAmount) 
                post.CommentCount -= decreaseAmount;
            else if (post != null) 
                post.CommentCount = 0;
        }

        _commentRepository.Remove(comment);
        await _commentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LikeCommentAsync(string userId, int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Comment not found.");

        var existingLike = await _commentRepository.GetLikeAsync(userId, commentId);

        if (existingLike != null)
        {
            // Đã like → unlike
            _commentRepository.RemoveLike(existingLike);
            if (comment.LikeCount > 0) comment.LikeCount--;
            await _commentRepository.SaveChangesAsync();
            return false;
        }

        var newLike = new LikeComment
        {
            UserId = userId,
            CommentId = commentId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddLikeAsync(newLike);
        comment.LikeCount++;
        await _commentRepository.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            comment.UserId,
            userId,
            NotificationType.Like,
            "đã thích bình luận của bạn",
            comment.PostId,
            comment.Id
        );

        return true;
    }

    public async Task<bool> UnlikeLikeCommentAsync(string userId, int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Comment not found.");

        var existingLike = await _commentRepository.GetLikeAsync(userId, commentId);

        if (existingLike == null)
        {
            // Chưa like → like luôn
            var newLike = new LikeComment
            {
                UserId = userId,
                CommentId = commentId,
                CreatedAt = DateTime.UtcNow
            };
            await _commentRepository.AddLikeAsync(newLike);
            comment.LikeCount++;
            await _commentRepository.SaveChangesAsync();
            return false;
        }

        _commentRepository.RemoveLike(existingLike);
        if (comment.LikeCount > 0) comment.LikeCount--;
        await _commentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdminDeleteCommentAsync(int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException("Comment not found.");

        var post = await _commentRepository.GetPostByIdAsync(comment.PostId);

        await _commentRepository.RemoveAllLikesByCommentIdAsync(commentId);
        await _commentRepository.RemoveNotificationsByCommentIdAsync(commentId);
        if (comment.ParentCommentId.HasValue)
        {
            var parent = await _commentRepository.GetByIdAsync(comment.ParentCommentId.Value);
            if (parent != null && parent.ReplyCount > 0) parent.ReplyCount--;
            if (post != null && post.CommentCount > 0) post.CommentCount--;
        }
        else
        {
            var decreaseAmount = 1 + comment.ReplyCount;
            if (post != null && post.CommentCount >= decreaseAmount) 
                post.CommentCount -= decreaseAmount;
            else if (post != null) 
                post.CommentCount = 0;
        }

        _commentRepository.Remove(comment);

        await _commentRepository.SaveChangesAsync();
        return true;
    }
}
