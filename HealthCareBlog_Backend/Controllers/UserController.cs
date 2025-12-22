using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IReportService _reportService;

        public UserController(IUserService userService, IReportService reportService)
        {
            _userService = userService;
            _reportService = reportService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] SignupDTO signupDTO)
        {
            var result = await _userService.SignupAsync(signupDTO);
            return Ok(new { message = "User registered successfully. Please check your email to verify your account.", success = result });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _userService.LoginAsync(loginDTO);
            return Ok(new { message = "Login successful.", token });
        }

        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _userService.VerifyEmailAsync(userId, token);
            return Ok(new { message = "Email verified successfully. You can now login.", success = result });
        }

        [HttpPost("resend-verification")]
        public async Task<ActionResult> ResendVerificationEmail([FromBody] ForgotPasswordDTO model)
        {
            var result = await _userService.ResendVerificationEmailAsync(model.Email);
            return Ok(new { message = "Verification email sent successfully.", success = result });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = await _userService.ForgotPasswordAsync(forgotPasswordDTO);
            return Ok(new { message = "If the email exists, a password reset link has been sent.", success = result });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordDTO);
            return Ok(new { message = "Password reset successfully. You can now login with your new password.", success = result });
        }

        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = User.GetUserId();
            var profile = await _userService.GetUserProfileAsync(currentUserId, userId, page, pageSize);
            return Ok(profile);
        }

        [HttpGet("account")]
        [Authorize]
        public async Task<ActionResult<ViewAccountDTO>> GetAccountInfo()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var account = await _userService.GetAccountInfoAsync(userId);
            return Ok(account);
        }

        [HttpPut("account")]
        [Authorize]
        public async Task<ActionResult<ViewAccountDTO>> UpdateAccountInfo([FromBody] UpdateAccountDTO updateAccountDTO)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var account = await _userService.UpdateAccountInfoAsync(userId, updateAccountDTO);
            return Ok(new { message = "Account updated successfully.", data = account });
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAccount()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _userService.DeleteUserAsync(userId);
            return Ok(new { message = "Account deleted successfully.", success = result });
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            return Ok(new { message = "User deleted successfully.", success = result });
        }

        [HttpPost("{userId}/report")]
        [Authorize]
        public async Task<ActionResult<ViewReportDTO>> ReportUser(string userId, [FromBody] CreateReportDTO createReportDTO)
        {
            var reporterId = User.GetUserId();
            if (string.IsNullOrEmpty(reporterId))
            {
                return Unauthorized("User not authenticated.");
            }

            createReportDTO.ContentType = "User";
            createReportDTO.ContentId = userId;

            var report = await _reportService.CreateReportAsync(reporterId, createReportDTO);
            return Ok(new { message = "User reported successfully.", data = report });
        }
    }
}
