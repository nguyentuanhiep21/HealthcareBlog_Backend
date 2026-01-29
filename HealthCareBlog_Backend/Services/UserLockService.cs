using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    /// <summary>
    /// Service để kiểm tra và quản lý lock status của user
    /// Hỗ trợ tự động mở khóa khi hạn khóa qua
    /// </summary>
    public class UserLockService : IUserLockService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Entities.User> _userManager;
        private readonly ILogger<UserLockService> _logger;

        public UserLockService(
            ApplicationDbContext context,
            UserManager<Models.Entities.User> userManager,
            ILogger<UserLockService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra và tự động mở khóa tài khoản nếu hạn khóa đã qua
        /// </summary>
        public async Task<bool> CheckAndUnlockExpiredLockAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                // Nếu không bị khóa, return false
                if (!user.IsLocked)
                    return false;

                // Nếu khóa vĩnh viễn (UnlockDate = null), giữ nguyên
                if (!user.UnlockDate.HasValue)
                    return false;

                var now = DateTime.UtcNow;

                // Nếu unlock date chưa đến, giữ nguyên khóa
                if (user.UnlockDate > now)
                    return false;

                // ✅ Hạn khóa đã qua, tự động mở khóa
                user.IsLocked = false;
                user.UnlockDate = null;
                user.LockedAt = null;
                user.LockReason = "Auto-unlocked - Lock period expired";

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to auto-unlock user {userId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return false;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Auto-unlocked user {user.UserName} (ID: {userId}) - Lock period expired");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking expired lock for user {userId}");
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin lock status của user (bao gồm unlock date còn lại)
        /// </summary>
        public async Task<UserLockStatusDto> GetUserLockStatusAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new UserLockStatusDto
                    {
                        IsLocked = false,
                        UnlockDate = null,
                        LockedAt = null,
                        LockReason = null,
                        DaysRemaining = null
                    };
                }

                // Nếu không bị khóa
                if (!user.IsLocked)
                {
                    return new UserLockStatusDto
                    {
                        IsLocked = false,
                        UnlockDate = null,
                        LockedAt = null,
                        LockReason = null,
                        DaysRemaining = null
                    };
                }

                // Khóa vĩnh viễn (UnlockDate = null)
                if (!user.UnlockDate.HasValue)
                {
                    return new UserLockStatusDto
                    {
                        IsLocked = true,
                        UnlockDate = null,
                        LockedAt = user.LockedAt,
                        LockReason = user.LockReason,
                        DaysRemaining = null // Vĩnh viễn
                    };
                }

                // Khóa có thời hạn
                var now = DateTime.UtcNow;
                var daysRemaining = user.UnlockDate.HasValue
                    ? (int)Math.Ceiling((user.UnlockDate.Value - now).TotalDays)
                    : 0;

                // Nếu hạn đã qua, return IsLocked = true nhưng DaysRemaining <= 0
                // Caller có thể gọi CheckAndUnlockExpiredLockAsync để mở khóa
                return new UserLockStatusDto
                {
                    IsLocked = user.IsLocked && daysRemaining > 0, // IsLocked = false nếu hạn đã qua
                    UnlockDate = user.UnlockDate,
                    LockedAt = user.LockedAt,
                    LockReason = user.LockReason,
                    DaysRemaining = daysRemaining > 0 ? daysRemaining : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting lock status for user {userId}");
                return new UserLockStatusDto
                {
                    IsLocked = false,
                    UnlockDate = null,
                    LockedAt = null,
                    LockReason = null,
                    DaysRemaining = null
                };
            }
        }
    }
}
