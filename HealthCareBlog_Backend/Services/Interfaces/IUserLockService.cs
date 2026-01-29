using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    /// <summary>
    /// Interface cho service kiểm tra và quản lý lock status của user
    /// </summary>
    public interface IUserLockService
    {
        /// <summary>
        /// Kiểm tra và tự động mở khóa tài khoản nếu hạn khóa đã qua
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>True nếu user được mở khóa, False nếu vẫn bị khóa hoặc không bị khóa</returns>
        Task<bool> CheckAndUnlockExpiredLockAsync(string userId);

        /// <summary>
        /// Lấy thông tin lock status của user (bao gồm unlock date còn lại)
        /// </summary>
        Task<UserLockStatusDto> GetUserLockStatusAsync(string userId);
    }

    public class UserLockStatusDto
    {
        public bool IsLocked { get; set; }
        public DateTime? UnlockDate { get; set; }
        public DateTime? LockedAt { get; set; }
        public string? LockReason { get; set; }
        public int? DaysRemaining { get; set; } // Số ngày còn lại cho tới khi mở khóa
    }
}
