namespace HealthCareBlog_Backend.Models.DTOs.Search
{
    public class SearchResultDTO
    {
        public List<SearchPostResultDTO> Posts { get; set; } = new List<SearchPostResultDTO>();
        public List<SearchUserResultDTO> Users { get; set; } = new List<SearchUserResultDTO>();
        public int TotalPosts { get; set; }
        public int TotalUsers { get; set; }
    }
}
