using System.Text.Json;
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
    private readonly ISupabaseStorageService _supabaseStorageService;

    public PostService(IPostRepository postRepository, INotificationService notificationService, ISupabaseStorageService supabaseStorageService)
    {
        _postRepository = postRepository;
        _notificationService = notificationService;
        _supabaseStorageService = supabaseStorageService;
    }

    /// <summary>
    /// Parse danh sách URL ảnh từ JSON string. Trả về list rỗng nếu không có.
    /// </summary>
    private static List<string> ParseImageUrls(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch { return new List<string>(); }
    }

    /// <summary>
    /// Build danh sách URL ảnh từ DTO (ưu tiên ImageUrls, fallback sang ImageUrl).
    /// Giới hạn tối đa 5 ảnh.
    /// </summary>
    private static List<string> ResolveImageUrls(List<string>? imageUrls, string? imageUrl)
    {
        if (imageUrls != null && imageUrls.Count > 0)
            return imageUrls.Take(5).ToList();
        if (!string.IsNullOrEmpty(imageUrl))
            return new List<string> { imageUrl };
        return new List<string>();
    }

    /// <summary>
    /// Xóa ảnh khỏi Supabase Storage nếu URL là Supabase URL (có chứa supabase).
    /// </summary>
    private async Task DeleteImagesFromStorageAsync(IEnumerable<string> urls)
    {
        foreach (var url in urls.Where(u => !string.IsNullOrEmpty(u)))
        {
            try
            {
                // Chỉ xóa file trên Supabase (URL chứa "supabase")
                if (url.Contains("supabase", StringComparison.OrdinalIgnoreCase))
                {
                    await _supabaseStorageService.DeleteFileAsync(url);
                }
                else
                {
                    // Fallback: xóa file local
                    FileHelper.DeletePostImage(url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostService] Warning: could not delete image {url}: {ex.Message}");
            }
        }
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

        // Resolve danh sách ảnh từ DTO
        var imageUrls = ResolveImageUrls(createPostDTO.ImageUrls, createPostDTO.ImageUrl);

        var newPost = new Post
        {
            UserId = authorId,
            Content = createPostDTO.Content,
            ImageUrl = imageUrls.FirstOrDefault() ?? string.Empty, // backward compat: ảnh đầu tiên
            ImageUrls = imageUrls.Count > 0 ? JsonSerializer.Serialize(imageUrls) : null,
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

        // Resolve danh sách ảnh mới từ DTO
        var newImageUrls = ResolveImageUrls(updatePostDTO.ImageUrls, updatePostDTO.ImageUrl);
        var oldImageUrls = ParseImageUrls(post.ImageUrls);

        // Nếu ImageUrls cũ rỗng, fallback sang ImageUrl cũ
        if (oldImageUrls.Count == 0 && !string.IsNullOrEmpty(post.ImageUrl))
            oldImageUrls = new List<string> { post.ImageUrl };

        // Xóa các ảnh cũ không còn trong danh sách mới
        var removedUrls = oldImageUrls.Except(newImageUrls).ToList();
        if (removedUrls.Count > 0)
            await DeleteImagesFromStorageAsync(removedUrls);

        // Cập nhật post
        post.Content = updatePostDTO.Content;
        post.ImageUrl = newImageUrls.FirstOrDefault(); // backward compat
        post.ImageUrls = newImageUrls.Count > 0 ? JsonSerializer.Serialize(newImageUrls) : null;

        await _postRepository.SaveChangesAsync();
        return post.ToPostDetailDTO(authorId);
    }

    public async Task<bool> DeletePostAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
            ?? throw new NotFoundException("Post not found.");

        // Xóa tất cả ảnh của bài viết
        var imageUrls = ParseImageUrls(post.ImageUrls);
        if (imageUrls.Count == 0 && !string.IsNullOrEmpty(post.ImageUrl))
            imageUrls = new List<string> { post.ImageUrl };

        if (imageUrls.Count > 0)
            await DeleteImagesFromStorageAsync(imageUrls);

        // Xóa likes của post
        await _postRepository.RemoveAllLikesByPostIdAsync(postId);

        // Xóa các dữ liệu liên đới (Comments, SavedPosts, Notifications)
        await _postRepository.RemoveRelatedDataByPostIdAsync(postId);

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

