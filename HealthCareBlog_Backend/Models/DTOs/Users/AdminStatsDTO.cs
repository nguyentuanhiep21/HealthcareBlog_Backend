namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    public class AdminStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int PendingReports { get; set; }
        public int LockedUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int NewPostsToday { get; set; }
        
        // Detailed report stats
        public int UserReports { get; set; }
        public int PostReports { get; set; }
        public int CommentReports { get; set; }
        public int ResolvedReports { get; set; }
        public int RejectedReports { get; set; }
        
        // Active users (not locked)
        public int ActiveUsers { get; set; }
    }
}
