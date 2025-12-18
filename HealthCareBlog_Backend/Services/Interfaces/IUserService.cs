using HealthCareBlog_Backend.Models.DTOs.Users;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> SignupAsync(SignupDTO signupDTO);
    }
}
