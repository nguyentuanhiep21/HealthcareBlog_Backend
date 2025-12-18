using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Users;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace HealthCareBlog_Backend.Services
{
    public class UserService : IUserService
    {
        public readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Entities.User> _userManager;

        public UserService(ApplicationDbContext context, UserManager<Models.Entities.User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            
            return true;
        }
    }
}
