# ?? H??NG D?N C?U HÌNH G?I EMAIL - SMARTCAMPUS HAU

## ?? M?C L?C
1. [T?ng quan](#t?ng-quan)
2. [C?u trúc Email Service](#c?u-trúc-email-service)
3. [C?u hình trong appsettings.json](#c?u-hình-appsettingsjson)
4. [??ng ký Service](#??ng-ký-service)
5. [Template HTML Email](#template-html-email)
6. [Các ?i?m quan tr?ng ?? tránh l?i font](#các-?i?m-quan-tr?ng)
7. [Troubleshooting](#troubleshooting)

---

## ?? T?NG QUAN

Project SmartCampus HAU s? d?ng `System.Net.Mail.SmtpClient` ?? g?i email HTML v?i h? tr? ti?ng Vi?t có d?u.

**Công ngh?:**
- .NET 8
- ASP.NET Core Identity
- System.Net.Mail
- UTF-8 Encoding

---

## ?? C?U TRÚC EMAIL SERVICE

### 1. Interface IEmailService

```csharp
// Services/Interfaces/IEmailService.cs
using System.Threading.Tasks;

namespace SmartCampus_HAU_Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
```

### 2. Implementation EmailService

```csharp
// Services/EmailService.cs
using SmartCampus_HAU_Backend.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SmartCampus_HAU_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // C?u hình SMTP Client
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]),
                EnableSsl = true,  // ?? B?T BU?C cho b?o m?t
            };

            // T?o mail message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,  // ? QUAN TR?NG NH?T - Render HTML ?úng
                BodyEncoding = System.Text.Encoding.UTF8,      // ? H? tr? ti?ng Vi?t
                SubjectEncoding = System.Text.Encoding.UTF8,   // ? H? tr? ti?ng Vi?t trong subject
            };
            
            mailMessage.To.Add(toEmail);

            // G?i email
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
```

**?? Các ?i?m quan tr?ng:**
- ? `IsBodyHtml = true` - B?T BU?C ?? render HTML
- ? `BodyEncoding = UTF8` - H? tr? ti?ng Vi?t
- ? `SubjectEncoding = UTF8` - Tiêu ?? ti?ng Vi?t
- ? `EnableSsl = true` - B?o m?t k?t n?i

---

## ?? C?U HÌNH APPSETTINGS.JSON

### appsettings.json ho?c appsettings.Development.json

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com"
  },
  "Frontend": {
    "ResetPasswordUrl": "http://localhost:5173/reset-password"
  }
}
```

### ?? L?u ý cho Gmail:

**Cách t?o App Password cho Gmail:**
1. Vào Google Account ? Security
2. B?t "2-Step Verification"
3. Vào "App Passwords"
4. Ch?n "Mail" và "Windows Computer"
5. Copy m?t kh?u 16 ký t? ? Dán vào `Password`

**Port thông d?ng:**
- Port **587**: TLS/STARTTLS (khuyên dùng)
- Port **465**: SSL

### Các SMTP Server khác:

```json
// Outlook/Hotmail
{
  "SmtpServer": "smtp-mail.outlook.com",
  "Port": "587"
}

// Yahoo
{
  "SmtpServer": "smtp.mail.yahoo.com",
  "Port": "587"
}

// Custom Domain (cPanel)
{
  "SmtpServer": "mail.yourdomain.com",
  "Port": "587"
}
```

---

## ?? ??NG KÝ SERVICE

### Program.cs

```csharp
// Program.cs
using SmartCampus_HAU_Backend.Services;
using SmartCampus_HAU_Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ... Các service khác ...

// ??ng ký Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// ... Middleware pipeline ...

app.Run();
```

---

## ?? TEMPLATE HTML EMAIL

### 1. Email Xác Nh?n Tài Kho?n

```csharp
private async Task SendEmailConfirmationAsync(User user)
{
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    var confirmationLink = $"https://localhost:7072/api/User/user/confirm-email?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

    var emailSubject = "Xác nh?n tài kho?n SmartCampus HAU";
    var emailBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #2c3e50;'>Chào m?ng ??n v?i SmartCampus HAU!</h2>
            <p>Xin chào <strong>{user.FullName}</strong>,</p>
            <p>C?m ?n b?n ?ã ??ng ký tài kho?n t?i SmartCampus HAU. ?? hoàn t?t quá trình ??ng ký, vui lòng xác nh?n email c?a b?n b?ng cách nh?p vào liên k?t bên d??i:</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{confirmationLink}' 
                   style='background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                    Xác nh?n Email
                </a>
            </div>
            
            <p><strong>Thông tin tài kho?n:</strong></p>
            <ul>
                <li><strong>Tên ??ng nh?p:</strong> {user.UserName}</li>
                <li><strong>Email:</strong> {user.Email}</li>
                <li><strong>H? tên:</strong> {user.FullName}</li>
            </ul>
            
            <p style='color: #e74c3c; font-size: 14px;'>
                <strong>L?u ý:</strong> Liên k?t này s? h?t h?n sau 24 gi?. N?u b?n không th?c hi?n vi?c ??ng ký này, vui lòng b? qua email này.
            </p>
            
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
            <p style='font-size: 12px; color: #7f8c8d;'>
                Email này ???c g?i t? ??ng t? h? th?ng SmartCampus HAU. Vui lòng không tr? l?i email này.
            </p>
        </div>";

    await _emailService.SendEmailAsync(user.Email!, emailSubject, emailBody);
}
```

### 2. Email ??t L?i M?t Kh?u

```csharp
public async Task<ServiceResult> SendForgotPasswordEmailAsync(ForgotPasswordRequest request)
{
    var email = request.Email;
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null || !user.EmailConfirmed)
    {
        return ServiceResult.Failure("Email không t?n t?i ho?c ch?a xác nh?n");
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetLink = $"https://localhost:7072/api/User/user/verify-reset-token?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

    var subject = "??t l?i m?t kh?u";
    var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #e74c3c;'>??t l?i m?t kh?u SmartCampus HAU</h2>
            <p>Xin chào <strong>{user.FullName}</strong>,</p>
            <p>Chúng tôi nh?n ???c yêu c?u ??t l?i m?t kh?u cho tài kho?n c?a b?n t?i SmartCampus HAU. ?? ti?p t?c, vui lòng nh?p vào nút bên d??i:</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' 
                   style='background-color: #e74c3c; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                    ??t l?i m?t kh?u
                </a>
            </div>
            
            <p style='color: #e74c3c; font-size: 14px;'>
                <strong>L?u ý:</strong> Liên k?t này s? h?t h?n sau 1 gi? vì lý do b?o m?t. N?u b?n không yêu c?u ??t l?i m?t kh?u, vui lòng b? qua email này và tài kho?n c?a b?n s? v?n an toàn.
            </p>
            
            <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 4px; padding: 15px; margin: 20px 0;'>
                <p style='margin: 0; color: #856404; font-size: 14px;'>
                    <strong>?? B?o m?t:</strong> N?u b?n không yêu c?u ??t l?i m?t kh?u, có th? ai ?ó ?ang c? g?ng truy c?p tài kho?n c?a b?n. Vui lòng liên h? b? ph?n h? tr? ngay l?p t?c.
                </p>
            </div>
            
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
            <p style='font-size: 12px; color: #7f8c8d;'>
                Email này ???c g?i t? ??ng t? h? th?ng SmartCampus HAU. Vui lòng không tr? l?i email này.
                <br>N?u c?n h? tr?, vui lòng liên h?: support@smartcampus-hau.edu.vn
            </p>
        </div>";

    await _emailService.SendEmailAsync(email, subject, body);
    return ServiceResult.Success("Email ??t l?i m?t kh?u ?ã ???c g?i. Vui lòng ki?m tra email.");
}
```

### 3. Email Thông Báo ??i M?t Kh?u

```csharp
private async Task SendPasswordChangedNotificationAsync(User user)
{
    var subject = "M?t kh?u ?ã ???c thay ??i - SmartCampus HAU";
    var emailBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #27ae60;'>M?t kh?u ?ã ???c thay ??i thành công</h2>
            <p>Xin chào <strong>{user.FullName}</strong>,</p>
            <p>M?t kh?u cho tài kho?n SmartCampus HAU c?a b?n ?ã ???c thay ??i thành công vào lúc:</p>
            
            <div style='background-color: #e8f5e8; border-left: 4px solid #27ae60; padding: 15px; margin: 20px 0;'>
                <p style='margin: 0;'><strong>Th?i gian:</strong> {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} (UTC+7)</p>
                <p style='margin: 5px 0 0 0;'><strong>Tài kho?n:</strong> {user.UserName} ({user.Email})</p>
            </div>
            
            <p>N?u <strong>b?n không th?c hi?n</strong> thay ??i này, vui lòng:</p>
            <ul>
                <li>??ng nh?p ngay ?? ki?m tra tài kho?n</li>
                <li>Liên h? b? ph?n h? tr?: support@smartcampus-hau.edu.vn</li>
                <li>Thay ??i m?t kh?u m?i ngay l?p t?c</li>
            </ul>
            
            <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 4px; padding: 15px; margin: 20px 0;'>
                <p style='margin: 0; color: #856404; font-size: 14px;'>
                    <strong>?? L?i khuyên b?o m?t:</strong> S? d?ng m?t kh?u m?nh có ít nh?t 8 ký t?, bao g?m ch? hoa, ch? th??ng, s? và ký t? ??c bi?t.
                </p>
            </div>
            
            <hr style='margin: 30px 0; border: none; border-top: 1px solid #ecf0f1;'>
            <p style='font-size: 12px; color: #7f8c8d;'>
                Email này ???c g?i t? ??ng t? h? th?ng SmartCampus HAU. Vui lòng không tr? l?i email này.
            </p>
        </div>";

    await _emailService.SendEmailAsync(user.Email!, subject, emailBody);
}
```

---

## ? CÁC ?I?M QUAN TR?NG ?? TRÁNH L?I FONT

### ? SAI - Không render HTML

```csharp
var mailMessage = new MailMessage
{
    Body = body,
    IsBodyHtml = false,  // ? SAI - Text thu?n, không có style
};
```

### ? ?ÚNG - Render HTML v?i UTF-8

```csharp
var mailMessage = new MailMessage
{
    From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
    Subject = subject,
    Body = body,
    IsBodyHtml = true,                              // ? Render HTML
    BodyEncoding = System.Text.Encoding.UTF8,       // ? UTF-8 cho body
    SubjectEncoding = System.Text.Encoding.UTF8,    // ? UTF-8 cho subject
};
```

### ?? Quy t?c vi?t HTML cho Email

#### 1. S? d?ng Inline CSS
```html
<!-- ? ?ÚNG -->
<div style='font-family: Arial, sans-serif; max-width: 600px;'>
    <h2 style='color: #2c3e50;'>Tiêu ??</h2>
</div>

<!-- ? SAI -->
<style>
    .container { font-family: Arial; }
</style>
<div class='container'>
    <h2>Tiêu ??</h2>
</div>
```

#### 2. Font-family an toàn
```css
/* ? T?T - Web-safe fonts */
font-family: Arial, sans-serif;
font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
font-family: Georgia, 'Times New Roman', serif;

/* ? TRÁNH - Custom fonts */
font-family: 'Roboto', sans-serif;  /* Có th? không load ???c */
```

#### 3. S? d?ng Single Quotes trong HTML String
```csharp
// ? ?ÚNG
var emailBody = $@"
    <div style='font-family: Arial, sans-serif;'>
        <h2 style='color: #2c3e50;'>Tiêu ??</h2>
    </div>";

// ? SAI - Conflict v?i double quotes c?a C#
var emailBody = $@"
    <div style=""font-family: Arial, sans-serif;"">
        <h2 style=""color: #2c3e50;"">Tiêu ??</h2>
    </div>";
```

#### 4. Max-width cho Container
```html
<div style='max-width: 600px; margin: 0 auto;'>
    <!-- Email content -->
</div>
```

#### 5. Color s? d?ng Hex code
```css
/* ? ?ÚNG */
color: #2c3e50;
background-color: #3498db;

/* ? TRÁNH */
color: rgb(44, 62, 80);
color: blue;
```

---

## ?? CHECKLIST TRI?N KHAI

```
? IsBodyHtml = true
? BodyEncoding = UTF8
? SubjectEncoding = UTF8
? EnableSsl = true
? Font-family: Arial, sans-serif
? Inline CSS cho t?t c? elements
? Single quotes trong HTML string
? Max-width: 600px cho container
? Port ?úng (587 cho Gmail TLS)
? App Password cho Gmail (không dùng m?t kh?u th??ng)
? File .cs l?u v?i UTF-8 encoding
? URI.EscapeDataString() cho URL parameters
```

---

## ?? TROUBLESHOOTING

### V?n ?? 1: Font ti?ng Vi?t hi?n th? sai (???? ho?c ???)

**Nguyên nhân:**
- `IsBodyHtml = false`
- Thi?u `BodyEncoding = UTF8`
- File không l?u d??i d?ng UTF-8

**Gi?i pháp:**
```csharp
var mailMessage = new MailMessage
{
    Body = body,
    IsBodyHtml = true,                          // ? B?t bu?c
    BodyEncoding = System.Text.Encoding.UTF8,   // ? B?t bu?c
    SubjectEncoding = System.Text.Encoding.UTF8, // ? B?t bu?c
};
```

### V?n ?? 2: Email không có style (màu s?c, font)

**Nguyên nhân:**
- `IsBodyHtml = false`
- S? d?ng `<style>` tag thay vì inline CSS

**Gi?i pháp:**
- ??t `IsBodyHtml = true`
- Dùng inline CSS: `style='color: #2c3e50;'`

### V?n ?? 3: Gmail t? ch?i ??ng nh?p

**Nguyên nhân:**
- S? d?ng m?t kh?u Google th??ng
- Ch?a b?t 2-Step Verification
- Ch?a t?o App Password

**Gi?i pháp:**
1. B?t 2-Step Verification t?i: https://myaccount.google.com/security
2. T?o App Password t?i: https://myaccount.google.com/apppasswords
3. S? d?ng App Password (16 ký t?) trong config

### V?n ?? 4: SMTP Connection Failed

**Nguyên nhân:**
- Port sai
- EnableSsl không ?úng
- Firewall ch?n

**Gi?i pháp:**
```json
// Gmail
{
  "SmtpServer": "smtp.gmail.com",
  "Port": "587",  // TLS
  "EnableSsl": true
}

// Ho?c
{
  "SmtpServer": "smtp.gmail.com",
  "Port": "465",  // SSL
  "EnableSsl": true
}
```

### V?n ?? 5: Link trong email b? l?i

**Nguyên nhân:**
- Không escape URL parameters
- Token ch?a ký t? ??c bi?t

**Gi?i pháp:**
```csharp
// ? ?ÚNG
var confirmationLink = $"https://localhost:7072/api/User/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

// ? SAI
var confirmationLink = $"https://localhost:7072/api/User/confirm-email?email={user.Email}&token={token}";
```

---

## ?? CODE TEST ??N GI?N

```csharp
// Controller ho?c Service
public async Task<IActionResult> TestEmail()
{
    var emailBody = @"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #2c3e50;'>Test Email ????</h2>
            <p>Xin chào! ?ây là email test.</p>
            <p>Ti?ng Vi?t có d?u: <strong>à á ? ã ? â ? ? ê ô ? ?</strong></p>
            <p>TI?NG VI?T HOA: <strong>À Á ? Ã ? Â ? ? Ê Ô ? ?</strong></p>
            
            <div style='background-color: #e8f5e8; padding: 15px; margin: 20px 0;'>
                <p style='margin: 0;'>? N?u b?n th?y d?u ti?ng Vi?t ?úng ? C?u hình OK!</p>
            </div>
            
            <ul>
                <li>Item 1: C?m ?n</li>
                <li>Item 2: Xin chào</li>
                <li>Item 3: Chúc m?ng n?m m?i ??</li>
            </ul>
        </div>";

    try
    {
        await _emailService.SendEmailAsync(
            "test@email.com", 
            "Test Email Ti?ng Vi?t", 
            emailBody
        );
        
        return Ok("Email ?ã ???c g?i! Ki?m tra inbox.");
    }
    catch (Exception ex)
    {
        return BadRequest($"L?i: {ex.Message}");
    }
}
```

---

## ?? TÀI LI?U THAM KH?O

### Dependencies c?n thi?t:
```xml
<!-- .csproj -->
<ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
    <!-- System.Net.Mail ?ã có s?n trong .NET Framework/Core -->
</ItemGroup>
```

### Namespace c?n import:
```csharp
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
```

---

## ?? K?T LU?N

### Nh?ng ?i?u B?T BU?C ph?i có:

1. ? `IsBodyHtml = true`
2. ? `BodyEncoding = System.Text.Encoding.UTF8`
3. ? `SubjectEncoding = System.Text.Encoding.UTF8`
4. ? Inline CSS trong HTML
5. ? Font-family an toàn (Arial, sans-serif)
6. ? Single quotes cho style attributes
7. ? App Password cho Gmail

### Best Practices:

- ?? Luôn wrap email trong container max-width 600px
- ?? S? d?ng web-safe colors (hex codes)
- ?? EnableSsl = true cho b?o m?t
- ?? Uri.EscapeDataString() cho URL parameters
- ?? Test email trên nhi?u email clients (Gmail, Outlook, Yahoo)

---

**Tác gi?:** SmartCampus HAU Team  
**Phiên b?n:** 1.0  
**Ngày c?p nh?t:** 2024  
**Công ngh?:** .NET 8, ASP.NET Core Identity

---

## ?? H? TR?

N?u g?p v?n ??, vui lòng ki?m tra:
1. Checklist bên trên
2. Troubleshooting section
3. Code test ??n gi?n ?? verify c?u hình

**Email h? tr?:** support@smartcampus-hau.edu.vn
