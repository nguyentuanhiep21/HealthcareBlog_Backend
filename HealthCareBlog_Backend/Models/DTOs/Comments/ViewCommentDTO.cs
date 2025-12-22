namespace HealthCareBlog_Backend.Models.DTOs.Comments
{
    public class ViewCommentDTO
    {
        public int Id { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public CommentAuthorDTO Author { get; set; } = null!;
        public int PostId { get; set; }
        public DateTime UploadTime { get; set; }
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    public class CommentAuthorDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
