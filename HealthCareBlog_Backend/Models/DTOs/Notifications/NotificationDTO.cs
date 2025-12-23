namespace HealthCareBlog_Backend.Models.DTOs.Notifications
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public ActorDTO? Actor { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
    }

    public class ActorDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
