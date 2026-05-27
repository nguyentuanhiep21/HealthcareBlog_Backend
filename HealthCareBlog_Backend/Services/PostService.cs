using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Helpers;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// PostService — business logic cho Posts.
/// Phụ thuộc vào IPostRepository (không còn phụ thuộc trực tiếp ApplicationDbContext).
/// </summary>
public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly INotificationService _notificationService;

    public PostService(IPostRepository postRepository, INotificationService notificationService)
    {
        _postRepository = postRepository;
        _notificationService = notificationService;
    }

    public async Task<PostDetailDTO> GetPostByIdAsync(string? userId, int postId)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        var comments = post.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.ToViewCommentDTO(userId))
            .ToList();

        var postDTO = post.ToPostDetailDTO(userId);
        postDTO.Comments = comments;

        return postDTO;
    }

    public async Task<List<ViewPostDTO>> ViewPostAsync(string? userId, int page = 1, int pageSize = 10)
    {
        var posts = await _postRepository.GetPagedAsync(page, pageSize);
        return posts.Select(p => p.ToViewPostDTO(userId)).ToList();
    }

    public async Task<List<ViewPostDTO>> GetTrendingPostsAsync(string? userId)
    {
        var posts = await _postRepository.GetTrendingTodayAsync();
        return posts.Select(p => p.ToViewPostDTO(userId)).ToList();
    }

    public async Task<PostDetailDTO> CreatePostAsync(string authorId, CreatePostDTO createPostDTO)
    {
        if (createPostDTO == null)
            throw new BadRequestException("Dữ liệu không hợp lệ.");

        var user = await _postRepository.GetUserByIdAsync(authorId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var newPost = new Post
        {
            UserId = authorId,
            Content = createPostDTO.Content,
            ImageUrl = createPostDTO.ImageUrl ?? string.Empty,
            CreatedAt = DateTimeHelper.GetVietnamTime(),
            LikeCount = 0,
            CommentCount = 0
        };

        await _postRepository.AddAsync(newPost);
        user.PostCount++;
        await _postRepository.SaveChangesAsync();

        return newPost.ToPostDetailDTO(authorId);
    }

    public async Task<PostDetailDTO> UpdatePostAsync(string authorId, int postId, UpdatePostDTO updatePostDTO)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        if (post.UserId != authorId)
            throw new UnauthorizedException("You are not authorized to update this post.");

        // Xóa ảnh cũ nếu đổi ảnh mới
        if (!string.IsNullOrEmpty(post.ImageUrl) && post.ImageUrl != updatePostDTO.ImageUrl)
            FileHelper.DeletePostImage(post.ImageUrl);

        post.Content = updatePostDTO.Content;
        post.ImageUrl = updatePostDTO.ImageUrl;

        await _postRepository.SaveChangesAsync();
        return post.ToPostDetailDTO(authorId);
    }

    public async Task<bool> DeletePostAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        // Xóa ảnh nếu có
        if (!string.IsNullOrEmpty(post.ImageUrl))
            FileHelper.DeletePostImage(post.ImageUrl);

        // Xóa likes của post
        await _postRepository.RemoveAllLikesByPostIdAsync(postId);

        _postRepository.Remove(post);
        await _postRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LikePostAsync(string userId, int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        var existingLike = await _postRepository.GetLikeAsync(userId, postId);

        if (existingLike != null)
        {
            // Đã like → unlike
            _postRepository.RemoveLike(existingLike);
            if (post.LikeCount > 0) post.LikeCount--;
            await _postRepository.SaveChangesAsync();
            return false;
        }

        var newLike = new LikePost
        {
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddLikeAsync(newLike);
        post.LikeCount++;
        await _postRepository.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            post.UserId, userId, NotificationType.Like, "đã thích bài viết của bạn", postId);

        return true;
    }

    public async Task<bool> UnlikePostAsync(string userId, int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        var existingLike = await _postRepository.GetLikeAsync(userId, postId);

        if (existingLike == null)
        {
            // Chưa like → like luôn
            var newLike = new LikePost
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };
            await _postRepository.AddLikeAsync(newLike);
            post.LikeCount++;
            await _postRepository.SaveChangesAsync();
            return false;
        }

        _postRepository.RemoveLike(existingLike);
        if (post.LikeCount > 0) post.LikeCount--;
        await _postRepository.SaveChangesAsync();
        return true;
    }
}
