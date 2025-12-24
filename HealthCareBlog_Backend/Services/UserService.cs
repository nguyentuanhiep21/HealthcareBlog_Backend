using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HealthCareBlog_Backend.Services
{
    public class UserService : IUserService
    {
        public readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Entities.User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserService(
            ApplicationDbContext context, 
            UserManager<Models.Entities.User> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<bool> SignupAsync(SignupDTO signupDTO)
        {
            if (signupDTO == null || string.IsNullOrEmpty(signupDTO.FullName) || string.IsNullOrWhiteSpace(signupDTO.Email)
                || string.IsNullOrWhiteSpace(signupDTO.Password) || string.IsNullOrWhiteSpace(signupDTO.PhoneNumber))
            {
                throw new BadRequestException("Dữ liệu không hợp lệ.");
            }
            
            var existingUser = await _userManager.FindByEmailAsync(signupDTO.Email);

            if (existingUser != null)
            {
                throw new BadRequestException("Email đã tồn tại.");
            }

            var newUser = new Models.Entities.User
            {
                FullName = signupDTO.FullName,
                UserName = signupDTO.Email,
                Email = signupDTO.Email,
                PhoneNumber = signupDTO.PhoneNumber,
                EmailConfirmed = false,
                AvatarUrl = "/images/logo.png", // Set default avatar
            };
            
            var result = await _userManager.CreateAsync(newUser, signupDTO.Password);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            await _emailService.SendVerificationEmailAsync(newUser.Email!, newUser.Id!, token);
            
            return true;
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                throw new BadRequestException("Dữ liệu không hợp lệ.");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedException("Email chưa được xác thực. Vui lòng xác thực email trước.");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");
            }

            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            if (user.EmailConfirmed)
            {
                throw new BadRequestException("Email đã được xác thực.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Mã xác thực không hợp lệ hoặc đã hết hạn.");
            }

            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            if (user.EmailConfirmed)
            {
                throw new BadRequestException("Email đã được xác thực.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendVerificationEmailAsync(user.Email!, user.Id!, token);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

            if (user == null)
            {
                return true;
            }

            if (!user.EmailConfirmed)
            {
                throw new BadRequestException("Email chưa được xác thực. Vui lòng xác thực email trước.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Id!, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Mã xác thực không hợp lệ hoặc đã hết hạn.");
            }

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            // Verify current password
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDTO.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                throw new BadRequestException("Mật khẩu hiện tại không chính xác.");
            }

            // Check if new password is same as current
            if (changePasswordDTO.CurrentPassword == changePasswordDTO.NewPassword)
            {
                throw new BadRequestException("Mật khẩu mới phải khác mật khẩu hiện tại.");
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Không thể đổi mật khẩu: {errors}");
            }

            return true;
        }

        public async Task<UserProfileDTO> GetUserProfileAsync(string? currentUserId, string userId, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var user = await _context.Users
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            var userProfile = user.ToUserProfileDTO(currentUserId);

            var posts = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.SavedByUsers)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            userProfile.Posts = posts.Select(p => p.ToViewPostDTO(currentUserId)).ToList();

            return userProfile;
        }

        public async Task<ViewAccountDTO> GetAccountInfoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            return user.ToViewAccountDTO();
        }

        public async Task<ViewAccountDTO> UpdateAccountInfoAsync(string userId, UpdateAccountDTO updateAccountDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            if (!string.IsNullOrWhiteSpace(updateAccountDTO.FullName))
            {
                user.FullName = updateAccountDTO.FullName;
                // Split FullName into FirstName and LastName
                var nameParts = updateAccountDTO.FullName.Trim().Split(new[] { ' ' }, 2);
                if (nameParts.Length == 2)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = nameParts[1];
                }
                else if (nameParts.Length == 1)
                {
                    user.FirstName = nameParts[0];
                    user.LastName = "";
                }
            }

            // Only update these fields if they are provided
            if (updateAccountDTO.PhoneNumber != null)
            {
                user.PhoneNumber = updateAccountDTO.PhoneNumber;
            }

            if (updateAccountDTO.Bio != null)
            {
                user.Bio = updateAccountDTO.Bio;
            }

            // Do NOT update avatar here - avatar is updated separately via UpdateAvatarAsync
            // if (!string.IsNullOrWhiteSpace(updateAccountDTO.AvatarUrl))
            // {
            //     user.AvatarUrl = updateAccountDTO.AvatarUrl;
            // }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user.ToViewAccountDTO();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            // Delete user avatar if exists
            FileHelper.DeleteAvatar(user.AvatarUrl);

            var userPosts = await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();
            
            foreach (var post in userPosts)
            {
                // Delete post image if exists
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    FileHelper.DeletePostImage(post.ImageUrl);
                }

                var postLikes = await _context.LikePosts
                    .Where(l => l.PostId == post.Id)
                    .ToListAsync();
                _context.LikePosts.RemoveRange(postLikes);

                var postComments = await _context.Comments
                    .Where(c => c.PostId == post.Id)
                    .ToListAsync();
                
                if (postComments.Any())
                {
                    var commentIds = postComments.Select(c => c.Id).ToList();
                    var commentLikes = await _context.LikeComments
                        .Where(l => commentIds.Contains(l.CommentId))
                        .ToListAsync();
                    _context.LikeComments.RemoveRange(commentLikes);
                    
                    var commentNotifications = await _context.Notifications
                        .Where(n => n.CommentId != null && commentIds.Contains(n.CommentId.Value))
                        .ToListAsync();
                    _context.Notifications.RemoveRange(commentNotifications);
                    
                    _context.Comments.RemoveRange(postComments);
                }

                var postNotifications = await _context.Notifications
                    .Where(n => n.PostId == post.Id)
                    .ToListAsync();
                _context.Notifications.RemoveRange(postNotifications);
            }
            _context.Posts.RemoveRange(userPosts);

            var userComments = await _context.Comments
                .Where(c => c.UserId == userId)
                .ToListAsync();
            
            if (userComments.Any())
            {
                var userCommentIds = userComments.Select(c => c.Id).ToList();
                var userCommentLikes = await _context.LikeComments
                    .Where(l => userCommentIds.Contains(l.CommentId))
                    .ToListAsync();
                _context.LikeComments.RemoveRange(userCommentLikes);
                
                var userCommentNotifications = await _context.Notifications
                    .Where(n => n.CommentId != null && userCommentIds.Contains(n.CommentId.Value))
                    .ToListAsync();
                _context.Notifications.RemoveRange(userCommentNotifications);
                
                _context.Comments.RemoveRange(userComments);
            }

            var userLikePosts = await _context.LikePosts
                .Where(l => l.UserId == userId)
                .ToListAsync();
            _context.LikePosts.RemoveRange(userLikePosts);

            var userLikeComments = await _context.LikeComments
                .Where(l => l.UserId == userId)
                .ToListAsync();
            _context.LikeComments.RemoveRange(userLikeComments);

            var userFollows = await _context.Follows
                .Where(f => f.FollowerId == userId || f.FollowingId == userId)
                .ToListAsync();
            _context.Follows.RemoveRange(userFollows);

            var userSavedPosts = await _context.SavedPosts
                .Where(sp => sp.UserId == userId)
                .ToListAsync();
            _context.SavedPosts.RemoveRange(userSavedPosts);

            var userNotifications = await _context.Notifications
                .Where(n => n.UserId == userId || n.ActorId == userId)
                .ToListAsync();
            _context.Notifications.RemoveRange(userNotifications);

            var userReports = await _context.ReportedContents
                .Where(r => r.ReporterId == userId)
                .ToListAsync();
            _context.ReportedContents.RemoveRange(userReports);

            var result = await _userManager.DeleteAsync(user);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SuggestedUserDTO>> GetSuggestedUsersAsync(string? currentUserId)
        {
            var users = await _context.Users
                .Include(u => u.Followers)
                .OrderByDescending(u => u.FollowerCount)
                .Take(3)
                .Select(u => new SuggestedUserDTO
                {
                    Id = u.Id!,
                    FullName = u.FullName ?? "",
                    AvatarUrl = u.AvatarUrl,
                    FollowerCount = u.FollowerCount,
                    IsFollowing = currentUserId != null && u.Followers.Any(f => f.FollowerId == currentUserId)
                })
                .ToListAsync();

            return users;
        }

        public async Task<ViewAccountDTO> UpdateAvatarAsync(string userId, string avatarUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Delete old avatar if it's not the default logo
            FileHelper.DeleteAvatar(user.AvatarUrl);

            user.AvatarUrl = avatarUrl;
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException("Failed to update avatar: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user.ToViewAccountDTO();
        }

        // Admin methods
        public async Task<List<AdminUserDTO>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? searchQuery = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerQuery = searchQuery.ToLower();
                query = query.Where(u => 
                    u.FullName!.ToLower().Contains(lowerQuery) || 
                    u.Email!.ToLower().Contains(lowerQuery) ||
                    u.UserName!.ToLower().Contains(lowerQuery));
            }

            var users = await query
                .OrderByDescending(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDTO
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
                    CreatedAt = DateTime.UtcNow, // You may want to add CreatedAt to User entity
                    LockedAt = u.LockedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<bool> ToggleUserLockAsync(string adminId, string userId, string? reason = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            if (user.IsAdmin)
            {
                throw new BadRequestException("Không thể khóa tài khoản admin.");
            }

            user.IsLocked = !user.IsLocked;
            user.LockedAt = user.IsLocked ? DateTime.UtcNow : null;
            user.LockReason = user.IsLocked ? reason : null;

            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException("Không thể cập nhật trạng thái người dùng.");
            }

            return true;
        }

        public async Task<AdminStatsDTO> GetAdminStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var totalUsers = await _context.Users.CountAsync();
            var lockedUsers = await _context.Users.CountAsync(u => u.IsLocked);

            var stats = new AdminStatsDTO
            {
                TotalUsers = totalUsers,
                TotalPosts = await _context.Posts.CountAsync(),
                TotalComments = await _context.Comments.CountAsync(),
                PendingReports = await _context.Reports.CountAsync(r => r.Status == "Pending"),
                LockedUsers = lockedUsers,
                ActiveUsers = totalUsers - lockedUsers,
                NewUsersToday = 0, // You may want to add CreatedAt to track this
                NewPostsToday = await _context.Posts.CountAsync(p => p.CreatedAt >= today),
                
                // Detailed report stats
                UserReports = await _context.Reports.CountAsync(r => r.ContentType == "User"),
                PostReports = await _context.Reports.CountAsync(r => r.ContentType == "Post"),
                CommentReports = await _context.Reports.CountAsync(r => r.ContentType == "Comment"),
                ResolvedReports = await _context.Reports.CountAsync(r => r.Status == "Resolved"),
                RejectedReports = await _context.Reports.CountAsync(r => r.Status == "Rejected")
            };

            return stats;
        }

        private async Task<string> GenerateJwtTokenAsync(Models.Entities.User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
}
