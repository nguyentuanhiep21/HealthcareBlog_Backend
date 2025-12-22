namespace HealthCareBlog_Backend.Helpers
{
    public static class DateTimeHelper
    {
        // GMT+7 Vietnam timezone
        public static DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
