using HealthCareBlog_Backend.Models.DTOs.Users;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> SignupAsync(SignupDTO signupDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<bool> VerifyEmailAsync(string userId, string token);
        Task<bool> ResendVerificationEmailAsync(string email);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
        Task<bool> DeleteUserAsync(string userId);
        Task<UserProfileDTO> GetUserProfileAsync(string? currentUserId, string userId, int page = 1, int pageSize = 10);
        Task<ViewAccountDTO> GetAccountInfoAsync(string userId);
        Task<ViewAccountDTO> UpdateAccountInfoAsync(string userId, UpdateAccountDTO updateAccountDTO);
        Task<List<SuggestedUserDTO>> GetSuggestedUsersAsync(string? currentUserId);
        Task<ViewAccountDTO> UpdateAvatarAsync(string userId, string avatarUrl);
        
        // Admin methods
        Task<List<AdminUserDTO>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? searchQuery = null);
        Task<bool> ToggleUserLockAsync(string adminId, string userId, string? reason = null);
        Task<AdminStatsDTO> GetAdminStatsAsync();
        Task<UserRolesDTO> GetUserRolesAsync(string userId);
        Task<UserRolesDTO> UpdateUserRolesAsync(string userId, List<string> roles);
    }
}
