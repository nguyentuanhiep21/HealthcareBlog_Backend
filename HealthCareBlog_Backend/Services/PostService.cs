using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class PostService : IPostService
    {
        public readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PostDetailDTO> CreatePostAsync(string AuthorId, CreatePostDTO createPostDTO)
        {
            if (createPostDTO == null)
            {
                throw new BadRequestException("Invalid data.");
            }

            var newPost = new Post
            {
                UserId = AuthorId,
                Content = createPostDTO.Content,
                ImageUrl = createPostDTO.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                LikeCount = 0,
                CommentCount = 0
            };
            
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return newPost.ToPostDetailDTO();
        }

        public async Task<PostDetailDTO> UpdatePostAsync(int postId, UpdatePostDTO updatePostDTO)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            post.Content = updatePostDTO.Content;
            post.ImageUrl = updatePostDTO.ImageUrl;

            await _context.SaveChangesAsync();
            return post.ToPostDetailDTO();
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            _context.Posts.Remove(post);
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

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.PostId == postId);

            if (existingLike != null)
            {
                throw new BadRequestException("You have already liked this post.");
            }

            var newLike = new Like
            {
                UserId = UserId,
                PostId = postId,
                CommentId = null, 
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(newLike);
            post.LikeCount++;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnlikePostAsync(string UserId, int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new NotFoundException("Post not found.");
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == UserId && l.PostId == postId);

            if (existingLike == null)
            {
                throw new BadRequestException("You have not liked this post yet.");
            }

            _context.Likes.Remove(existingLike);

            if (post.LikeCount > 0)
            {
                post.LikeCount--;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
