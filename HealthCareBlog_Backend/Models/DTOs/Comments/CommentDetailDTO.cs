namespace HealthCareBlog_Backend.Models.DTOs.Comments
{
    public class CommentDetailDTO
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string? UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
