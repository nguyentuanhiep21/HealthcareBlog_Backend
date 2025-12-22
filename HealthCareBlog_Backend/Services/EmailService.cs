using HealthCareBlog_Backend.Services.Interfaces;
using System.Net;
using System.Net.Mail;

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
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SmtpUsername"],
                    _configuration["EmailSettings:SmtpPassword"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]!),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendVerificationEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var verificationUrl = $"{frontendUrl}/auth/verify-email?userId={System.Uri.EscapeDataString(userId)}&token={System.Uri.EscapeDataString(token)}";

            var emailSubject = "Xac nhan tai khoan HealthCareBlog";
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Chao mung den voi HealthCareBlog!</h2>
                    <p>Xin chao,</p>
                    <p>Cam on ban da dang ky tai khoan tai HealthCareBlog. De hoan tat qua trinh dang ky, vui long xac nhan email cua ban bang cach nhap vao nut ben duoi:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{verificationUrl}' 
                           style='background-color: oklch(0.72 0.08 155); color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Xac nhan Email
                        </a>
                    </div>
                    
                    <p style='color: #e74c3c; font-size: 14px;'>
                        <strong>Luu y:</strong> Lien ket nay se het han sau 24 gio. Neu ban khong thuc hien viec dang ky nay, vui long bo qua email nay.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
                    <p style='font-size: 12px; color: #7f8c8d;'>
                        Email nay duoc gui tu dong tu he thong HealthCareBlog. Vui long khong tra loi email nay.
                    </p>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/auth/reset-password?userId={System.Uri.EscapeDataString(userId)}&token={System.Uri.EscapeDataString(token)}";

            var emailSubject = "Dat lai mat khau HealthCareBlog";
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Yeu cau dat lai mat khau</h2>
                    <p>Xin chao,</p>
                    <p>Chung toi nhan duoc yeu cau dat lai mat khau cho tai khoan cua ban. De tiep tuc, vui long nhap vao nut ben duoi:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' 
                           style='background-color: oklch(0.72 0.08 155); color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Dat lai mat khau
                        </a>
                    </div>
                    
                    <p style='color: #e74c3c; font-size: 14px;'>
                        <strong>Luu y:</strong> Lien ket nay chi co hieu luc trong 1 gio. Neu ban khong yeu cau dat lai mat khau, vui long bo qua email nay va tai khoan cua ban van an toan.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
                    <p style='font-size: 12px; color: #7f8c8d;'>
                        Email nay duoc gui tu dong tu he thong HealthCareBlog. Vui long khong tra loi email nay.
                    </p>
                </div>";

            await SendEmailAsync(email, emailSubject, emailBody);
        }
    }
}
