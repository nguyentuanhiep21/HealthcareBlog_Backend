namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task SendVerificationEmailAsync(string email, string userId, string token);
        Task SendPasswordResetEmailAsync(string email, string userId, string token);
    }
}
