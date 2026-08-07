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

            var emailSubject = "Xác nhận tài khoản HealthcareBlog";
            var emailBody = $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f8fafc; padding: 40px 20px; color: #334155;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);'>
                        <div style='background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%); padding: 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: 0.5px;'>HealthcareBlog</h1>
                        </div>
                        <div style='padding: 40px 30px;'>
                            <h2 style='color: #0f172a; font-size: 20px; font-weight: 600; margin-top: 0; margin-bottom: 20px;'>Chào mừng bạn đến với cộng đồng!</h2>
                            <p style='margin-bottom: 16px; font-size: 16px; line-height: 1.6;'>Xin chào,</p>
                            <p style='margin-bottom: 24px; font-size: 16px; line-height: 1.6;'>Cảm ơn bạn đã đăng ký tài khoản tại HealthCareBlog. Để hoàn tất quá trình đăng ký và bắt đầu chia sẻ kiến thức, vui lòng xác nhận địa chỉ email của bạn bằng cách nhấp vào nút bên dưới:</p>
                            
                            <div style='text-align: center; margin: 35px 0;'>
                                <a href='{verificationUrl}' 
                                   style='background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: 600; font-size: 16px; box-shadow: 0 4px 14px 0 rgba(8, 145, 178, 0.39);'>
                                    Xác Nhận Email
                                </a>
                            </div>
                            
                            <div style='background-color: #fff1f2; border-left: 4px solid #f43f5e; padding: 12px 16px; border-radius: 0 4px 4px 0; margin-bottom: 24px;'>
                                <p style='color: #be123c; font-size: 14px; margin: 0;'>
                                    <strong>Lưu ý:</strong> Liên kết này sẽ hết hạn sau 24 giờ. Nếu bạn không thực hiện việc đăng ký này, vui lòng bỏ qua email này.
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; line-height: 1.6; margin-bottom: 0;'>Trân trọng,<br>Đội ngũ HealthCareBlog</p>
                        </div>
                        <div style='background-color: #f1f5f9; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                            <p style='font-size: 13px; color: #64748b; margin: 0;'>
                                Email này được gửi tự động từ hệ thống HealthCareBlog. Vui lòng không trả lời email này.
                            </p>
                        </div>
                    </div>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/auth/reset-password?userId={System.Uri.EscapeDataString(userId)}&token={System.Uri.EscapeDataString(token)}";

            var emailSubject = "Đặt lại mật khẩu HealthcareBlog";
            var emailBody = $@"
                <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f8fafc; padding: 40px 20px; color: #334155;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);'>
                        <div style='background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%); padding: 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: 0.5px;'>HealthcareBlog</h1>
                        </div>
                        <div style='padding: 40px 30px;'>
                            <h2 style='color: #0f172a; font-size: 20px; font-weight: 600; margin-top: 0; margin-bottom: 20px;'>Đặt lại mật khẩu</h2>
                            <p style='margin-bottom: 16px; font-size: 16px; line-height: 1.6;'>Xin chào,</p>
                            <p style='margin-bottom: 24px; font-size: 16px; line-height: 1.6;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn tại HealthCareBlog. Để tiếp tục, vui lòng nhấp vào nút bên dưới để thiết lập mật khẩu mới:</p>
                            
                            <div style='text-align: center; margin: 35px 0;'>
                                <a href='{resetUrl}' 
                                   style='background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: 600; font-size: 16px; box-shadow: 0 4px 14px 0 rgba(8, 145, 178, 0.39);'>
                                    Đặt Lại Mật Khẩu
                                </a>
                            </div>
                            
                            <div style='background-color: #fff1f2; border-left: 4px solid #f43f5e; padding: 12px 16px; border-radius: 0 4px 4px 0; margin-bottom: 24px;'>
                                <p style='color: #be123c; font-size: 14px; margin: 0;'>
                                    <strong>Lưu ý bảo mật:</strong> Liên kết này sẽ hết hạn sau 1 giờ. Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này và tài khoản của bạn sẽ vẫn an toàn.
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; line-height: 1.6; margin-bottom: 0;'>Trân trọng,<br>Đội ngũ HealthCareBlog</p>
                        </div>
                        <div style='background-color: #f1f5f9; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                            <p style='font-size: 13px; color: #64748b; margin: 0;'>
                                Email này được gửi tự động từ hệ thống HealthCareBlog. Vui lòng không trả lời email này.
                            </p>
                        </div>
                    </div>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }
    }
}
