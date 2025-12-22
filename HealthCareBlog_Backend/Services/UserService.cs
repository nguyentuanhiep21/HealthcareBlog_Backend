using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Mapper;
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
                || string.IsNullOrWhiteSpace(signupDTO.Password) || string.IsNullOrWhiteSpace(signupDTO.Phone))
            {
                throw new BadRequestException("Invalid data.");
            }
            
            var existingUser = await _userManager.FindByEmailAsync(signupDTO.Email);

            if (existingUser != null)
            {
                throw new BadRequestException("Email is already in use.");
            }

            var newUser = new Models.Entities.User
            {
                FullName = signupDTO.FullName,
                UserName = signupDTO.Email,
                Email = signupDTO.Email,
                PhoneNumber = signupDTO.Phone,
                EmailConfirmed = false,
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
                throw new BadRequestException("Invalid data.");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedException("Email not verified. Please verify your email first.");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var token = await GenerateJwtTokenAsync(user);
            return token;
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (user.EmailConfirmed)
            {
                throw new BadRequestException("Email already verified.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Invalid or expired token.");
            }

            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (user.EmailConfirmed)
            {
                throw new BadRequestException("Email already verified.");
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
                throw new BadRequestException("Email not verified. Please verify your email first.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Id!, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Invalid or expired token.");
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
                throw new NotFoundException("User not found.");
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
                throw new NotFoundException("User not found.");
            }

            return user.ToViewAccountDTO();
        }

        public async Task<ViewAccountDTO> UpdateAccountInfoAsync(string userId, UpdateAccountDTO updateAccountDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.FirstName = updateAccountDTO.FirstName;
            user.LastName = updateAccountDTO.LastName;
            
            if (!string.IsNullOrWhiteSpace(updateAccountDTO.FirstName) && !string.IsNullOrWhiteSpace(updateAccountDTO.LastName))
            {
                user.FullName = $"{updateAccountDTO.FirstName} {updateAccountDTO.LastName}";
            }
            else if (!string.IsNullOrWhiteSpace(updateAccountDTO.FirstName))
            {
                user.FullName = updateAccountDTO.FirstName;
            }
            else if (!string.IsNullOrWhiteSpace(updateAccountDTO.LastName))
            {
                user.FullName = updateAccountDTO.LastName;
            }

            user.PhoneNumber = updateAccountDTO.PhoneNumber;
            user.Bio = updateAccountDTO.Bio;
            user.AvatarUrl = updateAccountDTO.AvatarUrl;

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
                throw new NotFoundException("User not found.");
            }

            var userPosts = await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();
            
            foreach (var post in userPosts)
            {
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
