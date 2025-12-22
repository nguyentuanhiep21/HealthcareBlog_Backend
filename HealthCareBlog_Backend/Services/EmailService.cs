using HealthCareBlog_Backend.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HealthCareBlog_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }

        public async Task SendVerificationEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var verificationUrl = $"{frontendUrl}/auth/verify-email?userId={userId}&token={WebUtility.UrlEncode(token)}";

            var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
            var logoBase64 = string.Empty;

            if (File.Exists(logoPath))
            {
                var logoBytes = await File.ReadAllBytesAsync(logoPath);
                logoBase64 = Convert.ToBase64String(logoBytes);
            }

            var htmlContent = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 40px 20px;
            text-align: center;
        }}
        .logo {{
            width: 80px;
            height: 80px;
            margin-bottom: 20px;
        }}
        .header h1 {{
            color: #ffffff;
            margin: 0;
            font-size: 28px;
        }}
        .content {{
            padding: 40px 30px;
            text-align: center;
        }}
        .content p {{
            color: #666;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            padding: 15px 40px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 25px;
            font-weight: bold;
            font-size: 16px;
            transition: transform 0.2s;
        }}
        .button:hover {{
            transform: scale(1.05);
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #999;
            font-size: 14px;
        }}
        .divider {{
            height: 1px;
            background-color: #e0e0e0;
            margin: 30px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            {(string.IsNullOrEmpty(logoBase64) ? "<div style='width: 80px; height: 80px; margin: 0 auto 20px; background-color: #4CAF50; border-radius: 50%;'></div>" : $"<img src='data:image/png;base64,{logoBase64}' alt='Logo' class='logo' />")}
            <h1>Xác Th?c Email</h1>
        </div>
        <div class='content'>
            <p>Xin chào,</p>
            <p>C?m ?n b?n ?ã ??ng ký tài kho?n HealthCareBlog. Vui lòng nh?n vào nút bên d??i ?? xác th?c ??a ch? email c?a b?n.</p>
            <a href='{verificationUrl}' class='button'>Xác Th?c Email</a>
            <div class='divider'></div>
            <p style='font-size: 14px; color: #999;'>
                N?u b?n không th? nh?n vào nút, vui lòng sao chép và dán liên k?t sau vào trình duy?t:<br>
                <span style='color: #667eea; word-break: break-all;'>{verificationUrl}</span>
            </p>
            <p style='font-size: 14px; color: #999;'>
                Link này s? h?t h?n sau 24 gi?.
            </p>
        </div>
        <div class='footer'>
            <p>© 2024 HealthCareBlog. All rights reserved.</p>
            <p>N?u b?n không yêu c?u email này, vui lòng b? qua.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, "Xác Th?c Email - HealthCareBlog", htmlContent);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/auth/reset-password?userId={userId}&token={WebUtility.UrlEncode(token)}";

            var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
            var logoBase64 = string.Empty;

            if (File.Exists(logoPath))
            {
                var logoBytes = await File.ReadAllBytesAsync(logoPath);
                logoBase64 = Convert.ToBase64String(logoBytes);
            }

            var htmlContent = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #81c784 0%, #66bb6a 100%);
            padding: 40px 20px;
            text-align: center;
        }}
        .logo {{
            width: 80px;
            height: 80px;
            margin-bottom: 20px;
        }}
        .header h1 {{
            color: #ffffff;
            margin: 0;
            font-size: 28px;
        }}
        .content {{
            padding: 40px 30px;
            text-align: center;
        }}
        .content p {{
            color: #666;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            padding: 15px 40px;
            background: linear-gradient(135deg, #81c784 0%, #66bb6a 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 25px;
            font-weight: bold;
            font-size: 16px;
            transition: transform 0.2s;
        }}
        .button:hover {{
            transform: scale(1.05);
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #999;
            font-size: 14px;
        }}
        .divider {{
            height: 1px;
            background-color: #e0e0e0;
            margin: 30px 0;
        }}
        .warning {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
            text-align: left;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            {(string.IsNullOrEmpty(logoBase64) ? "<div style='width: 80px; height: 80px; margin: 0 auto 20px; background-color: #4CAF50; border-radius: 50%;'></div>" : $"<img src='data:image/png;base64,{logoBase64}' alt='Logo' class='logo' />")}
            <h1>Quên M?t Kh?u</h1>
        </div>
        <div class='content'>
            <p>Xin chào,</p>
            <p>Chúng tôi nh?n ???c yêu c?u ??t l?i m?t kh?u cho tài kho?n c?a b?n. Nh?n vào nút bên d??i ?? ??t l?i m?t kh?u.</p>
            <a href='{resetUrl}' class='button'>??t L?i M?t Kh?u</a>
            <div class='warning'>
                <strong>?? L?u ý:</strong> Link này ch? có hi?u l?c trong 1 gi?. N?u b?n không yêu c?u ??t l?i m?t kh?u, vui lòng b? qua email này.
            </div>
            <div class='divider'></div>
            <p style='font-size: 14px; color: #999;'>
                N?u b?n không th? nh?n vào nút, vui lòng sao chép và dán liên k?t sau vào trình duy?t:<br>
                <span style='color: #66bb6a; word-break: break-all;'>{resetUrl}</span>
            </p>
        </div>
        <div class='footer'>
            <p>© 2024 HealthCareBlog. All rights reserved.</p>
            <p>N?u b?n không yêu c?u email này, tài kho?n c?a b?n v?n an toàn.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, "??t L?i M?t Kh?u - HealthCareBlog", htmlContent);
        }
    }
}
