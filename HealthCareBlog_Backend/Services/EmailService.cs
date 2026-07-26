using HealthCareBlog_Backend.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace HealthCareBlog_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var apiKey = _configuration["EmailSettings:SendGridApiKey"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:FromName"]);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                
                var response = await client.SendEmailAsync(msg);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Email sent successfully to: {toEmail}");
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"❌ Failed to send email to {toEmail}. SendGrid Error: {errorBody}");
                    throw new Exception($"SendGrid Error: {errorBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send email to {toEmail}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task SendVerificationEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var verificationUrl = $"{frontendUrl}/auth/verify-email?userId={System.Uri.EscapeDataString(userId)}&token={System.Uri.EscapeDataString(token)}";

            var emailSubject = "Xác nhận tài khoản HealthCareBlog";
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Chào mừng đến với HealthCareBlog!</h2>
                    <p>Xin chào,</p>
                    <p>Cảm ơn bạn đã đăng ký tài khoản tại HealthCareBlog. Để hoàn tất quá trình đăng ký, vui lòng xác nhận email của bạn bằng cách nhấp vào nút bên dưới:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{verificationUrl}' 
                           style='background-color: #6EC177; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Xác nhận Email
                        </a>
                    </div>
                    
                    <p style='color: #e74c3c; font-size: 14px;'>
                        <strong>Lưu ý:</strong> Liên kết này sẽ hết hạn sau 24 giờ. Nếu bạn không thực hiện việc đăng ký này, vui lòng bỏ qua email này.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
                    <p style='font-size: 12px; color: #7f8c8d;'>
                        Email này được gửi tự động từ hệ thống HealthCareBlog. Vui lòng không trả lời email này.
                    </p>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/auth/reset-password?userId={System.Uri.EscapeDataString(userId)}&token={System.Uri.EscapeDataString(token)}";

            var emailSubject = "Đặt lại mật khẩu HealthCareBlog";
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Đặt lại mật khẩu HealthCareBlog</h2>
                    <p>Xin chào,</p>
                    <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Để tiếp tục, vui lòng nhấp vào nút bên dưới:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' 
                           style='background-color: #6EC177; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Đặt lại mật khẩu
                        </a>
                    </div>
                    
                    <p style='color: #e74c3c; font-size: 14px;'>
                        <strong>Lưu ý:</strong> Liên kết này sẽ hết hạn sau 1 giờ vì lý do bảo mật. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này và tài khoản của bạn sẽ vẫn an toàn.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
                    <p style='font-size: 12px; color: #7f8c8d;'>
                        Email này được gửi tự động từ hệ thống HealthCareBlog. Vui lòng không trả lời email này.
                    </p>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }
    }
}
