namespace HealthCareBlog_Backend.Models.DTOs.SavedPosts
{
    public class ViewSavedPostDTO
    {
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}