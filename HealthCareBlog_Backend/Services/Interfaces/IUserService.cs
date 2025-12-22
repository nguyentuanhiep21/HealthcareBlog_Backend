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
        Task<bool> DeleteUserAsync(string userId);
        Task<UserProfileDTO> GetUserProfileAsync(string? currentUserId, string userId, int page = 1, int pageSize = 10);
        Task<ViewAccountDTO> GetAccountInfoAsync(string userId);
        Task<ViewAccountDTO> UpdateAccountInfoAsync(string userId, UpdateAccountDTO updateAccountDTO);
    }
}
