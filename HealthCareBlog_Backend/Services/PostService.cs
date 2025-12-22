using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using HealthCareBlog_Backend.Models.DTOs.Comments;
using HealthCareBlog_Backend.Helpers;

namespace HealthCareBlog_Backend.Services
{
    public class PostService : IPostService
    {
        public readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PostDetailDTO> GetPostByIdAsync(string? UserId, int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var comments = post.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.ToViewCommentDTO(UserId))
                .ToList();

            var postDTO = post.ToPostDetailDTO(UserId);
            postDTO.Comments = comments;

            return postDTO;
        }

        public async Task<List<ViewPostDTO>> ViewPostAsync(string? UserId, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            
            if(UserId != null)
            {
                var user = await _context.Users.FindAsync(UserId);
            }
            
            var posts = await _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Followers)
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            var postDTOs = posts.Select(p => p.ToViewPostDTO(UserId)).ToList();
            return postDTOs;
        }

        public async Task<PostDetailDTO> CreatePostAsync(string AuthorId, CreatePostDTO createPostDTO)
        {
            Console.WriteLine($"[CreatePostAsync] Starting - AuthorId: {AuthorId}");
            
            if (createPostDTO == null)
            {
                Console.WriteLine("[CreatePostAsync] Error: createPostDTO is null");
                throw new BadRequestException("Dữ liệu không hợp lệ.");
            }

            Console.WriteLine($"[CreatePostAsync] Content length: {createPostDTO.Content?.Length ?? 0}");
            Console.WriteLine($"[CreatePostAsync] ImageUrl length: {createPostDTO.ImageUrl?.Length ?? 0}");

            var user = await _context.Users.FindAsync(AuthorId);
            if (user == null)
            {
                Console.WriteLine($"[CreatePostAsync] Error: User not found - {AuthorId}");
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            Console.WriteLine($"[CreatePostAsync] User found: {user.FullName}");

            var newPost = new Post
            {
                UserId = AuthorId,
                Content = createPostDTO.Content,
                ImageUrl = createPostDTO.ImageUrl,
                CreatedAt = DateTimeHelper.GetVietnamTime(),
                LikeCount = 0,
                CommentCount = 0
            };
            
            Console.WriteLine("[CreatePostAsync] Adding post to context...");
            _context.Posts.Add(newPost);
            user.PostCount++;
            
            Console.WriteLine("[CreatePostAsync] Saving changes...");
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"[CreatePostAsync] Success - Post ID: {newPost.Id}");
            return newPost.ToPostDetailDTO(AuthorId);
        }

        public async Task<PostDetailDTO> UpdatePostAsync(string AuthorId, int postId, UpdatePostDTO updatePostDTO)
        {
            var post = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            if (post.UserId != AuthorId)
            {
                throw new UnauthorizedException("You are not authorized to update this post.");
            }

            post.Content = updatePostDTO.Content;
            post.ImageUrl = updatePostDTO.ImageUrl;

            await _context.SaveChangesAsync();
            return post.ToPostDetailDTO(AuthorId);
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var user = await _context.Users.FindAsync(post.UserId);

            // Delete image file if exists
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                try
                {
                    // Parse ImageUrl to get physical path
                    // ImageUrl format: /images/posts/filename.jpg
                    var imagePath = post.ImageUrl.TrimStart('/');
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
                    
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        Console.WriteLine($"[PostService] Deleted image file: {fullPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PostService] Error deleting image file: {ex.Message}");
                    // Continue with post deletion even if image deletion fails
                }
            }

            var postLikes = await _context.LikePosts
                .Where(l => l.PostId == postId)
                .ToListAsync();
            
            if (postLikes.Any())
            {
                _context.LikePosts.RemoveRange(postLikes);
            }

            var savedPosts = await _context.SavedPosts
                .Where(sp => sp.PostId == postId)
                .ToListAsync();
            
            if (savedPosts.Any())
            {
                _context.SavedPosts.RemoveRange(savedPosts);
            }

            var postComments = await _context.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync();
            
            if (postComments.Any())
            {
                var commentIds = postComments.Select(c => c.Id).ToList();
                
                var commentLikes = await _context.LikeComments
                    .Where(l => commentIds.Contains(l.CommentId))
                    .ToListAsync();
                
                if (commentLikes.Any())
                {
                    _context.LikeComments.RemoveRange(commentLikes);
                }

                var commentNotifications = await _context.Notifications
                    .Where(n => n.CommentId != null && commentIds.Contains(n.CommentId.Value))
                    .ToListAsync();
                
                if (commentNotifications.Any())
                {
                    _context.Notifications.RemoveRange(commentNotifications);
                }

                _context.Comments.RemoveRange(postComments);
            }

            var postNotifications = await _context.Notifications
                .Where(n => n.PostId == postId)
                .ToListAsync();
            
            if (postNotifications.Any())
            {
                _context.Notifications.RemoveRange(postNotifications);
            }

            _context.Posts.Remove(post);
            
            if (user != null && user.PostCount > 0)
            {
                user.PostCount--;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikePostAsync(string UserId, int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var existingLike = await _context.LikePosts
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.PostId == postId);

            if (existingLike != null)
            {
                // Đã like rồi, unlike luôn
                _context.LikePosts.Remove(existingLike);
                if (post.LikeCount > 0)
                {
                    post.LikeCount--;
                }
                await _context.SaveChangesAsync();
                return false; // Return false để biết là đã unlike
            }

            var newLike = new LikePost
            {
                UserId = UserId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            _context.LikePosts.Add(newLike);
            post.LikeCount++;
            await _context.SaveChangesAsync();

            return true; // Return true để biết là đã like
        }

        public async Task<bool> UnlikePostAsync(string UserId, int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var existingLike = await _context.LikePosts
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.PostId == postId);

            if (existingLike == null)
            {
                // Chưa like, like luôn
                var newLike = new LikePost
                {
                    UserId = UserId,
                    PostId = postId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.LikePosts.Add(newLike);
                post.LikeCount++;
                await _context.SaveChangesAsync();
                return false; // Return false để biết là đã like
            }

            _context.LikePosts.Remove(existingLike);

            if (post.LikeCount > 0)
            {
                post.LikeCount--;
            }

            await _context.SaveChangesAsync();
            return true; // Return true để biết là đã unlike
        }
    }
}
