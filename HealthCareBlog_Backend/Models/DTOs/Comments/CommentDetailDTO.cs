namespace HealthCareBlog_Backend.Models.DTOs.Comments
{
    public class CommentDetailDTO
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? UserId { get; set; }
        public CommentUserDTO User { get; set; } = null!;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
