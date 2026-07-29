namespace HealthCareBlog_Backend.Models.DTOs.Posts
{
    public class TrendingPostDTO
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorName { get; set; }
    }
}
