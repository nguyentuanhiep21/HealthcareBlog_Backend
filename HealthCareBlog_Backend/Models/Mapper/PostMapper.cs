using System.Text.Json;
using HealthCareBlog_Backend.Models.DTOs.Posts;
using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Models.Mapper
{
    public static class PostMapper
    {
        /// <summary>
        /// Parse ImageUrls JSON string từ entity thành List<string>.
        /// Fallback: nếu ImageUrls null/rỗng và ImageUrl có giá trị, dùng ImageUrl làm danh sách 1 phần tử.
        /// </summary>
        private static List<string> ParseImageUrls(Post post)
        {
            if (!string.IsNullOrEmpty(post.ImageUrls))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<List<string>>(post.ImageUrls);
                    if (parsed != null && parsed.Count > 0)
                        return parsed;
                }
                catch { /* ignore malformed JSON, fallback below */ }
            }

            // Backward compat: bài viết cũ chỉ có ImageUrl
            if (!string.IsNullOrEmpty(post.ImageUrl))
                return new List<string> { post.ImageUrl };

            return new List<string>();
        }

        public static PostDetailDTO ToPostDetailDTO(this Post post, string? userId = null)
        {
            var imageUrls = ParseImageUrls(post);
            return new PostDetailDTO
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = imageUrls.FirstOrDefault(), // backward compat: ảnh đầu tiên
                ImageUrls = imageUrls,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                AuthorId = post.UserId,
                IsLikedByCurrentUser = userId != null && post.Likes.Any(like => like.UserId == userId),
                IsSavedByCurrentUser = userId != null && post.SavedByUsers.Any(sp => sp.UserId == userId),
                Author = new AuthorDTO
                {
                    Id = post.User?.Id ?? "",
                    FullName = post.User?.FullName ?? "Unknown",
                    AvatarUrl = post.User?.AvatarUrl,
                }
            };
        }

        public static ViewPostDTO ToViewPostDTO(this Post post, string? userId)
        {
            var imageUrls = ParseImageUrls(post);
            return new ViewPostDTO
            {
                Id = post.Id,
                AuthorId = post.UserId,
                Content = post.Content,
                ImageUrl = imageUrls.FirstOrDefault(), // backward compat
                ImageUrls = imageUrls,
                UploadTime = post.CreatedAt,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLikedByCurrentUser = userId != null && post.Likes.Any(like => like.UserId == userId),
                IsSavedByCurrentUser = userId != null && post.SavedByUsers.Any(sp => sp.UserId == userId),
                Author = new AuthorDTO
                {
                    Id = post.User?.Id ?? "",
                    FullName = post.User?.FullName ?? "Unknown",
                    AvatarUrl = post.User?.AvatarUrl,
                    Bio = post.User?.Bio,
                    IsFollowing = userId != null && post.User != null && post.User.Followers.Any(f => f.FollowerId == userId)
                }
            };
        }
    }
}
