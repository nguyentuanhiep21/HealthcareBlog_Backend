using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
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
            try
            {
                var result = await _userService.SignupAsync(signupDTO);
                return Ok(new { message = "User registered successfully. Please check your email to verify your account.", success = result });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var token = await _userService.LoginAsync(loginDTO);
                return Ok(new { message = "Login successful.", token });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                var result = await _userService.VerifyEmailAsync(userId, token);
                return Ok(new { message = "Email verified successfully. You can now login.", success = result });
            }
            catch (BadRequestException ex)
            {
                if (ex.Message.Contains("already verified"))
                {
                    return Ok(new { message = "Email đã được xác thực trước đó. Bạn có thể đăng nhập ngay!", success = true });
                }
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", success = false });
            }
        }

        [HttpPost("resend-verification")]
        public async Task<ActionResult> ResendVerificationEmail([FromBody] ForgotPasswordDTO model)
        {
            try
            {
                var result = await _userService.ResendVerificationEmailAsync(model.Email);
                return Ok(new { message = "Verification email sent successfully.", success = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi gửi email. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                var result = await _userService.ForgotPasswordAsync(forgotPasswordDTO);
                return Ok(new { message = "If the email exists, a password reset link has been sent.", success = result });
            }
            catch (Exception)
            {
                return Ok(new { message = "If the email exists, a password reset link has been sent.", success = true });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(resetPasswordDTO);
                return Ok(new { message = "Password reset successfully. You can now login with your new password.", success = result });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
                return Ok(new { message = "Đổi mật khẩu thành công!", success = result });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = User.GetUserId();
                var profile = await _userService.GetUserProfileAsync(currentUserId, userId, page, pageSize);
                return Ok(profile);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("suggested")]
        public async Task<ActionResult<List<SuggestedUserDTO>>> GetSuggestedUsers()
        {
            try
            {
                var currentUserId = User.GetUserId();
                var suggestedUsers = await _userService.GetSuggestedUsersAsync(currentUserId);
                return Ok(suggestedUsers);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("account")]
        [Authorize]
        public async Task<ActionResult<ViewAccountDTO>> GetAccountInfo()
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var account = await _userService.GetAccountInfoAsync(userId);
                return Ok(account);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPut("account")]
        [Authorize]
        public async Task<ActionResult<ViewAccountDTO>> UpdateAccountInfo([FromBody] UpdateAccountDTO updateAccountDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var account = await _userService.UpdateAccountInfoAsync(userId, updateAccountDTO);
                return Ok(new { message = "Account updated successfully.", data = account });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAccount()
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var result = await _userService.DeleteUserAsync(userId);
                return Ok(new { message = "Account deleted successfully.", success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                return Ok(new { message = "User deleted successfully.", success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminUserDTO>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchQuery = null)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(page, pageSize, searchQuery);
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPut("{userId}/toggle-lock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ToggleUserLock(string userId, [FromBody] ToggleUserLockDTO? dto)
        {
            try
            {
                var adminId = User.GetUserId();
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var result = await _userService.ToggleUserLockAsync(adminId, userId, dto?.Reason);
                return Ok(new { message = "User lock status updated successfully.", success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpGet("admin/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminStatsDTO>> GetAdminStats()
        {
            try
            {
                var stats = await _userService.GetAdminStatsAsync();
                return Ok(stats);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        /// <summary>
        /// Get user roles (Swagger only - for testing/development)
        /// </summary>
        [HttpGet("{userId}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRolesDTO>> GetUserRoles(string userId)
        {
            try
            {
                var userRoles = await _userService.GetUserRolesAsync(userId);
                return Ok(userRoles);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        /// <summary>
        /// Update user roles (Swagger only - for testing/development)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/user/roles
        ///     {
        ///         "userId": "user-guid-here",
        ///         "roles": ["Admin", "User"]
        ///     }
        ///     
        /// Valid roles: Admin, User
        /// </remarks>
        [HttpPut("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRolesDTO>> UpdateUserRoles([FromBody] UpdateUserRolesDTO updateUserRolesDTO)
        {
            try
            {
                var userRoles = await _userService.UpdateUserRolesAsync(updateUserRolesDTO.UserId, updateUserRolesDTO.Roles);
                return Ok(new { message = "Cập nhật roles thành công.", success = true, data = userRoles });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }

        [HttpPut("avatar")]
        [Authorize]
        public async Task<ActionResult<ViewAccountDTO>> UpdateAvatar([FromBody] UpdateAvatarDTO updateAvatarDTO)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated.", success = false });
                }

                var updatedAccount = await _userService.UpdateAvatarAsync(userId, updateAvatarDTO.AvatarUrl);
                return Ok(updatedAccount);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }
        [HttpPost("{userId}/report")]
        [Authorize]
        public async Task<ActionResult<ViewReportDTO>> ReportUser(string userId, [FromBody] CreateUserReportDTO createUserReportDTO)
        {
            try
            {
                var reporterId = User.GetUserId();
                if (string.IsNullOrEmpty(reporterId))
                {
                    return Unauthorized(new { message = "Bạn chưa đăng nhập.", success = false });
                }

                var createReportDTO = new CreateReportDTO
                {
                    ContentType = "User",
                    ContentId = userId,
                    Reason = createUserReportDTO.Reason,
                    Description = createUserReportDTO.Description
                };

                var (report, isExisting) = await _reportService.CreateReportAsync(reporterId, createReportDTO);
                
                var message = isExisting 
                    ? "Bạn đã báo cáo người dùng này trước đó rồi." 
                    : "Báo cáo người dùng thành công.";
                
                return Ok(new { message, success = true, data = report });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi. Vui lòng thử lại sau.", success = false });
            }
        }
    }
}
