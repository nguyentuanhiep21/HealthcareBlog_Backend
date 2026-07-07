using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Helpers;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// UserService — business logic cho User module.
/// UserManager (Identity) giữ nguyên — đây là abstraction, không phải raw DB access.
/// ApplicationDbContext đã được thay thế hoàn toàn bằng IUserRepository.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IUserLockService _userLockService;

    public UserService(
        IUserRepository userRepository,
        IPostRepository postRepository,
        UserManager<User> userManager,
        IEmailService emailService,
        IConfiguration configuration,
        IUserLockService userLockService)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _userLockService = userLockService;
    }

    public async Task<bool> SignupAsync(SignupDTO signupDTO)
    {
        if (signupDTO == null || string.IsNullOrEmpty(signupDTO.FullName)
            || string.IsNullOrWhiteSpace(signupDTO.Email)
            || string.IsNullOrWhiteSpace(signupDTO.Password))
            throw new BadRequestException("Dữ liệu không hợp lệ.");

        var existingUser = await _userManager.FindByEmailAsync(signupDTO.Email);
        if (existingUser != null)
            throw new BadRequestException("Email đã tồn tại.");

        var newUser = new User
        {
            FullName = signupDTO.FullName,
            UserName = signupDTO.Email,
            Email = signupDTO.Email,
            PhoneNumber = signupDTO.PhoneNumber,
            EmailConfirmed = false,
            AvatarUrl = null,
        };

        var result = await _userManager.CreateAsync(newUser, signupDTO.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        await _emailService.SendVerificationEmailAsync(newUser.Email!, newUser.Id!, token);

        return true;
    }

    public async Task<string> LoginAsync(LoginDTO loginDTO)
    {
        if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email)
            || string.IsNullOrWhiteSpace(loginDTO.Password))
            throw new BadRequestException("Dữ liệu không hợp lệ.");

        var user = await _userManager.FindByEmailAsync(loginDTO.Email)
            ?? throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");

        if (!user.EmailConfirmed)
            throw new UnauthorizedException("Email chưa được xác thực. Vui lòng xác thực email trước.");

        // FAST LOCK CHECK — kiểm tra trạng thái khóa mà KHÔNG UPDATE (tránh chậm)
        if (user.IsLocked)
        {
            var now = DateTime.UtcNow;

            if (!user.UnlockDate.HasValue)
            {
                var lockReason = string.IsNullOrEmpty(user.LockReason) ? "Tài khoản đã bị khóa." : user.LockReason;
                throw new UnauthorizedException($"Tài khoản đã bị khóa. Lý do: {lockReason}");
            }

            if (user.UnlockDate > now)
            {
                var daysRemaining = (int)Math.Ceiling((user.UnlockDate.Value - now).TotalDays);
                throw new UnauthorizedException($"Tài khoản đã bị khóa tạm thời. Vui lòng thử lại sau {daysRemaining} ngày.");
            }

            // Hạn khóa đã qua → Cho phép đăng nhập, update ở background
            _ = _userLockService.CheckAndUnlockExpiredLockAsync(user.Id);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
        if (!isPasswordValid)
            throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");

        return await GenerateJwtTokenAsync(user);
    }

    public async Task<bool> VerifyEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        if (user.EmailConfirmed)
            throw new BadRequestException("Email đã được xác thực.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            throw new BadRequestException("Mã xác thực không hợp lệ hoặc đã hết hạn.");

        return true;
    }

    public async Task<bool> ResendVerificationEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        if (user.EmailConfirmed)
            throw new BadRequestException("Email đã được xác thực.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendVerificationEmailAsync(user.Email!, user.Id!, token);

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
        if (user == null) return true; // Không tiết lộ email có tồn tại không

        if (!user.EmailConfirmed)
            throw new BadRequestException("Email chưa được xác thực. Vui lòng xác thực email trước.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Id!, token);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException("Mã xác thực không hợp lệ hoặc đã hết hạn.");

        return true;
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var isCurrentValid = await _userManager.CheckPasswordAsync(user, changePasswordDTO.CurrentPassword);
        if (!isCurrentValid)
            throw new BadRequestException("Mật khẩu hiện tại không chính xác.");

        if (changePasswordDTO.CurrentPassword == changePasswordDTO.NewPassword)
            throw new BadRequestException("Mật khẩu mới phải khác mật khẩu hiện tại.");

        var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException($"Không thể đổi mật khẩu: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return true;
    }

    public async Task<UserProfileDTO> GetUserProfileAsync(string? currentUserId, string userId, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var user = await _userRepository.GetByIdWithFollowersAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var userProfile = user.ToUserProfileDTO(currentUserId);

        var posts = await _postRepository.GetByUserIdAsync(userId, page, pageSize);
        userProfile.Posts = posts.Select(p => p.ToViewPostDTO(currentUserId)).ToList();

        return userProfile;
    }

    public async Task<ViewAccountDTO> GetAccountInfoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        return await user.ToViewAccountDTOAsync(_userManager);
    }

    public async Task<ViewAccountDTO> UpdateAccountInfoAsync(string userId, UpdateAccountDTO updateAccountDTO)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        if (!string.IsNullOrWhiteSpace(updateAccountDTO.FullName))
        {
            user.FullName = updateAccountDTO.FullName;
            var nameParts = updateAccountDTO.FullName.Trim().Split(new[] { ' ' }, 2);
            user.FirstName = nameParts[0];
            user.LastName = nameParts.Length == 2 ? nameParts[1] : "";
        }

        if (updateAccountDTO.Bio != null)
            user.Bio = updateAccountDTO.Bio;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return await user.ToViewAccountDTOAsync(_userManager);
    }

    public async Task<ViewAccountDTO> UpdateAvatarAsync(string userId, string avatarUrl)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (!string.IsNullOrEmpty(user.AvatarUrl))
            FileHelper.DeleteAvatar(user.AvatarUrl);

        user.AvatarUrl = avatarUrl;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException("Failed to update avatar: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        return await user.ToViewAccountDTOAsync(_userManager);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        // Xóa avatar nếu có
        if (!string.IsNullOrEmpty(user.AvatarUrl))
            FileHelper.DeleteAvatar(user.AvatarUrl);

        // Lấy posts của user
        var userPosts = await _userRepository.GetUserPostsWithDetailsAsync(userId);
        var postIds = userPosts.Select(p => p.Id).ToList();

        if (postIds.Any())
        {
            // Xóa ảnh post
            foreach (var post in userPosts)
                if (!string.IsNullOrEmpty(post.ImageUrl))
                    FileHelper.DeletePostImage(post.ImageUrl);

            // Cascade: likes → comment likes → comment notifications → comments → post notifications
            await _userRepository.RemovePostLikesByPostIdsAsync(postIds);

            var commentIds = await _userRepository.GetCommentIdsByUserIdAsync(userId);
            if (commentIds.Any())
            {
                await _userRepository.RemoveCommentLikesByCommentIdsAsync(commentIds);
                await _userRepository.RemoveNotificationsByCommentIdsAsync(commentIds);
            }

            await _userRepository.RemoveCommentsByPostIdsAsync(postIds);
            await _userRepository.RemoveNotificationsByPostIdsAsync(postIds);
            await _userRepository.RemovePostsByUserIdAsync(userId);
        }

        // Xóa dữ liệu user còn lại
        await _userRepository.RemoveCommentsByUserIdAsync(userId);
        await _userRepository.RemovePostLikesByUserIdAsync(userId);
        await _userRepository.RemoveCommentLikesByUserIdAsync(userId);
        await _userRepository.RemoveFollowsByUserIdAsync(userId);
        await _userRepository.RemoveSavedPostsByUserIdAsync(userId);
        await _userRepository.RemoveNotificationsByUserIdAsync(userId);
        await _userRepository.ClearReporterReferenceAsync(userId);

        // Save trước khi xóa Identity user
        await _userRepository.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return true;
    }

    public async Task<List<SuggestedUserDTO>> GetSuggestedUsersAsync(string? currentUserId)
        => await _userRepository.GetSuggestedUsersAsync(currentUserId);

    public async Task<List<AdminUserDTO>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? searchQuery = null)
    {
        var users = await _userRepository.GetPagedAsync(page, pageSize, searchQuery);
        return users.Select(u => new AdminUserDTO
        {
            Id = u.Id,
            Username = u.UserName ?? "",
            Email = u.Email ?? "",
            FullName = u.FullName,
            Bio = u.Bio,
            AvatarUrl = u.AvatarUrl,
            FollowersCount = u.FollowerCount,
            FollowingCount = u.FollowingCount,
            PostsCount = u.PostCount,
            IsLocked = u.IsLocked,
            CreatedAt = DateTime.UtcNow,
            LockedAt = u.LockedAt
        }).ToList();
    }

    public async Task<bool> LockUserAsync(string adminId, string userId, string? reason = null, DateTime? unlockDate = null)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
            throw new BadRequestException("Không thể khóa tài khoản admin.");

        user.IsLocked = true;
        user.LockedAt = DateTime.UtcNow;
        user.LockReason = reason;
        user.UnlockDate = unlockDate;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException("Không thể cập nhật trạng thái người dùng.");

        return true;
    }

    public async Task<bool> UnlockUserAsync(string adminId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        user.IsLocked = false;
        user.LockedAt = null;
        user.LockReason = null;
        user.UnlockDate = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException("Không thể cập nhật trạng thái người dùng.");

        return true;
    }

    public async Task<AdminStatsDTO> GetAdminStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var totalUsers = await _userRepository.CountUsersAsync();
        var lockedUsers = await _userRepository.CountLockedUsersAsync();

        return new AdminStatsDTO
        {
            TotalUsers = totalUsers,
            TotalPosts = await _userRepository.CountPostsAsync(),
            TotalComments = await _userRepository.CountCommentsAsync(),
            PendingReports = await _userRepository.CountPendingReportsAsync(),
            LockedUsers = lockedUsers,
            ActiveUsers = totalUsers - lockedUsers,
            NewUsersToday = 0,
            NewPostsToday = await _userRepository.CountPostsTodayAsync(today),
            UserReports = await _userRepository.CountReportsByContentTypeAsync("User"),
            PostReports = await _userRepository.CountReportsByContentTypeAsync("Post"),
            CommentReports = await _userRepository.CountReportsByContentTypeAsync("Comment"),
            ResolvedReports = await _userRepository.CountReportsByStatusAsync("Resolved"),
            RejectedReports = await _userRepository.CountReportsByStatusAsync("Rejected")
        };
    }

    // ========== PRIVATE ==========

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName ?? user.Email!)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtSettings:ExpirationDays"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
