namespace HealthCareBlog_Backend.Models.DTOs.Comments
{
    public class CreateCommentDTO
    {
        public int? ParentCommentId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;

    }
}
