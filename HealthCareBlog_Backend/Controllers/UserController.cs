using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// UserController — chỉ xử lý HTTP request/response.
/// GlobalExceptionHandlerMiddleware bắt toàn bộ lỗi, không cần try/catch ở đây.
/// </summary>
[Route("api/users")]
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
    public async Task<ActionResult> Signup([FromBody] SignupDTO dto)
    {
        var result = await _userService.SignupAsync(dto);
        return Ok(new { message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.", success = result });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO dto)
    {
        var token = await _userService.LoginAsync(dto);
        return Ok(new { message = "Đăng nhập thành công.", token });
    }

    [HttpGet("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
    {
        await _userService.VerifyEmailAsync(userId, token);
        return Ok(new { message = "Xác thực email thành công. Bạn có thể đăng nhập ngay!", success = true });
    }

    [HttpPost("resend-verification")]
    public async Task<ActionResult> ResendVerificationEmail([FromBody] ForgotPasswordDTO model)
    {
        await _userService.ResendVerificationEmailAsync(model.Email);
        return Ok(new { message = "Email xác thực đã được gửi lại.", success = true });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
    {
        await _userService.ForgotPasswordAsync(dto);
        return Ok(new { message = "Nếu email tồn tại, liên kết đặt lại mật khẩu đã được gửi.", success = true });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        await _userService.ResetPasswordAsync(dto);
        return Ok(new { message = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập ngay!", success = true });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        var userId = User.GetUserId()!;
        await _userService.ChangePasswordAsync(userId, dto);
        return Ok(new { message = "Đổi mật khẩu thành công!", success = true });
    }

    [HttpGet("profile/{userId}")]
    public async Task<ActionResult<UserProfileDTO>> GetUserProfile(
        string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var currentUserId = User.GetUserId();
        var profile = await _userService.GetUserProfileAsync(currentUserId, userId, page, pageSize);
        return Ok(profile);
    }

    [HttpGet("suggested")]
    public async Task<ActionResult<List<SuggestedUserDTO>>> GetSuggestedUsers()
    {
        var currentUserId = User.GetUserId();
        var users = await _userService.GetSuggestedUsersAsync(currentUserId);
        return Ok(users);
    }

    [HttpGet("account")]
    [Authorize]
    public async Task<ActionResult<ViewAccountDTO>> GetAccountInfo()
    {
        var userId = User.GetUserId()!;
        var account = await _userService.GetAccountInfoAsync(userId);
        return Ok(account);
    }

    [HttpPut("account")]
    [Authorize]
    public async Task<ActionResult<ViewAccountDTO>> UpdateAccountInfo([FromBody] UpdateAccountDTO dto)
    {
        var userId = User.GetUserId()!;
        var account = await _userService.UpdateAccountInfoAsync(userId, dto);
        return Ok(new { message = "Cập nhật thông tin thành công.", data = account });
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteAccount()
    {
        var userId = User.GetUserId()!;
        await _userService.DeleteUserAsync(userId);
        return Ok(new { message = "Xóa tài khoản thành công.", success = true });
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(string userId)
    {
        await _userService.DeleteUserAsync(userId);
        return Ok(new { message = "Xóa người dùng thành công.", success = true });
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AdminUserDTO>>> GetAllUsers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? searchQuery = null)
    {
        var users = await _userService.GetAllUsersAsync(page, pageSize, searchQuery);
        return Ok(users);
    }

    [HttpPut("{userId}/toggle-lock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ToggleUserLock(string userId, [FromBody] LockUserRequest? request)
    {
        var adminId = User.GetUserId()!;

        if (request?.UnlockDate.HasValue == true && request.UnlockDate <= DateTime.UtcNow)
            return BadRequest(new { message = "Ngày mở khóa phải là ngày trong tương lai.", success = false });

        await _userService.LockUserAsync(adminId, userId, request?.Reason, request?.UnlockDate);
        return Ok(new { message = "Tài khoản người dùng đã bị khóa.", success = true });
    }

    [HttpPut("{userId}/unlock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnlockUser(string userId)
    {
        var adminId = User.GetUserId()!;
        await _userService.UnlockUserAsync(adminId, userId);
        return Ok(new { message = "Mở khóa tài khoản thành công.", success = true });
    }

    [HttpGet("admin/stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminStatsDTO>> GetAdminStats()
    {
        var stats = await _userService.GetAdminStatsAsync();
        return Ok(stats);
    }

    [HttpPut("avatar")]
    [Authorize]
    public async Task<ActionResult<ViewAccountDTO>> UpdateAvatar([FromBody] UpdateAvatarDTO dto)
    {
        var userId = User.GetUserId()!;
        var account = await _userService.UpdateAvatarAsync(userId, dto.AvatarUrl);
        return Ok(account);
    }

    [HttpPut("banner")]
    [Authorize]
    public async Task<ActionResult<ViewAccountDTO>> UpdateBanner([FromBody] UpdateBannerDTO dto)
    {
        var userId = User.GetUserId()!;
        var account = await _userService.UpdateBannerAsync(userId, dto.BannerUrl);
        return Ok(account);
    }

    [HttpPost("{userId}/report")]
    [Authorize]
    public async Task<ActionResult<ViewReportDTO>> ReportUser(string userId, [FromBody] CreateUserReportDTO dto)
    {
        var reporterId = User.GetUserId()!;
        var createReportDTO = new CreateReportDTO
        {
            ContentType = "User",
            ContentId = userId,
            Reason = dto.Reason,
            Description = dto.Description
        };

        var (report, isExisting) = await _reportService.CreateReportAsync(reporterId, createReportDTO);
        var message = isExisting ? "Bạn đã báo cáo người dùng này trước đó rồi." : "Báo cáo người dùng thành công.";

        return Ok(new { message, success = true, data = report });
    }
}
