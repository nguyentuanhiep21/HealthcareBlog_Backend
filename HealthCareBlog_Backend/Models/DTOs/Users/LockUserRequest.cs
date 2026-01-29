namespace HealthCareBlog_Backend.Models.DTOs.Users
{
    /// <summary>
    /// Request DTO để khóa tài khoản người dùng
    /// </summary>
    public class LockUserRequest
    {
        /// <summary>
        /// Lý do khóa tài khoản (tùy chọn)
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Ngày tự động mở khóa (tùy chọn)
        /// - Nếu null = khóa vĩnh viễn
        /// - Nếu có value = khóa tạm thời đến ngày này
        /// </summary>
        public DateTime? UnlockDate { get; set; }

        /// <summary>
        /// Kiểm tra xem có hợp lệ không
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // Nếu có UnlockDate, phải là ngày trong tương lai
            if (UnlockDate.HasValue && UnlockDate <= DateTime.UtcNow)
                return false;

            return true;
        }
    }
}
