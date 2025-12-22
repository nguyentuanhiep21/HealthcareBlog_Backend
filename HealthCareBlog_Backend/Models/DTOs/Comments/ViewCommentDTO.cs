namespace HealthCareBlog_Backend.Models.DTOs.Comments
{
    public class ViewCommentDTO
    {
        public string AuthorId { get; set; } = string.Empty;
        public int PostId { get; set; }
        public DateTime UploadTime { get; set; }
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
