# HealthCareBlog Backend - H??ng D?n Authentication

## C?u Hình

### 1. Email Settings (appsettings.json)

C?u hình SMTP ?? g?i email xác th?c và ??t l?i m?t kh?u:

```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "HealthCareBlog"
}
```

**L?u ý**: V?i Gmail, c?n t?o App Password thay vì s? d?ng m?t kh?u th??ng.

### 2. Frontend URL

Trong `appsettings.json`, c?p nh?t URL c?a frontend:

```json
"AppSettings": {
  "FrontendUrl": "http://localhost:3000"
}
```

**TODO**: Khi deploy production, thay ??i thành domain th?c t? c?a frontend.

### 3. JWT Settings

```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMustBeLongEnough",
  "Issuer": "HealthCareBlog",
  "Audience": "HealthCareBlogUsers",
  "ExpirationDays": 7
}
```

## API Endpoints

### Authentication

#### 1. ??ng Ký (Signup)
```http
POST /api/User/signup
Content-Type: application/json

{
  "fullName": "Nguyen Van A",
  "email": "user@example.com",
  "phone": "0123456789",
  "password": "Password@123"
}
```

**Response:**
- Email xác th?c s? ???c g?i ??n ??a ch? email ??ng ký
- Link xác th?c: `{FrontendUrl}/auth/verify-email?userId={userId}&token={token}`

#### 2. ??ng Nh?p (Login)
```http
POST /api/User/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password@123"
}
```

**Response:**
```json
{
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**L?u ý**: Ch? ??ng nh?p ???c sau khi xác th?c email.

#### 3. Xác Th?c Email
```http
GET /api/User/verify-email?userId={userId}&token={token}
```

**Frontend Route**: `/auth/verify-email`
- Frontend nh?n `userId` và `token` t? query string
- G?i API backend ?? xác th?c
- Hi?n th? thông báo thành công/th?t b?i

#### 4. G?i L?i Email Xác Th?c
```http
POST /api/User/resend-verification
Content-Type: application/json

{
  "email": "user@example.com"
}
```

#### 5. Quên M?t Kh?u
```http
POST /api/User/forgot-password
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Response:**
- Email ??t l?i m?t kh?u s? ???c g?i
- Link ??t l?i: `{FrontendUrl}/auth/reset-password?userId={userId}&token={token}`

#### 6. ??t L?i M?t Kh?u
```http
POST /api/User/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "token": "CfDJ8...",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}
```

**Frontend Route**: `/auth/reset-password`
- Frontend nh?n `userId` và `token` t? query string
- Hi?n th? form nh?p m?t kh?u m?i
- G?i API backend v?i email, token và m?t kh?u m?i

### Profile & Account

#### 7. Xem Trang Cá Nhân
```http
GET /api/User/profile/{userId}?page=1&pageSize=10
```

#### 8. Xem Thông Tin Tài Kho?n
```http
GET /api/User/account
Authorization: Bearer {token}
```

#### 9. C?p Nh?t Thông Tin Tài Kho?n
```http
PUT /api/User/account
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "Van",
  "lastName": "Nguyen",
  "phoneNumber": "0987654321",
  "bio": "Healthcare blogger",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

## Frontend Implementation Guide

### Routes C?n T?o

1. **`/auth/verify-email`**
   - Nh?n `userId` và `token` t? URL query
   - G?i `GET /api/User/verify-email`
   - Hi?n th? loading ? success/error message
   - Redirect v? login sau 3 giây n?u thành công

2. **`/auth/reset-password`**
   - Nh?n `userId` và `token` t? URL query
   - Hi?n th? form nh?p m?t kh?u m?i
   - Validate m?t kh?u (min 8 ký t?, có ch? hoa, th??ng, s?, ký t? ??c bi?t)
   - G?i `POST /api/User/reset-password`
   - Redirect v? login sau khi thành công

3. **`/auth/forgot-password`**
   - Hi?n th? form nh?p email
   - G?i `POST /api/User/forgot-password`
   - Hi?n th? thông báo ?ã g?i email

### Authentication Flow

```
1. User ??ng ký ? Email xác th?c ???c g?i
2. User click link trong email ? Frontend route /auth/verify-email
3. Frontend g?i API verify ? Hi?n th? k?t qu?
4. User ??ng nh?p ? Nh?n JWT token
5. L?u token vào localStorage/sessionStorage
6. G?i token trong header: Authorization: Bearer {token}
```

### Error Handling

```typescript
// Example error responses
{
  "message": "Email not verified. Please verify your email first."
}

{
  "message": "Invalid email or password."
}

{
  "message": "Invalid or expired token."
}
```

## Database Migration

Sau khi c?u hình xong, ch?y migration ?? c?p nh?t database:

```bash
dotnet ef database update --project HealthCareBlog_Backend
```

## Testing

### Test Email Local (v?i MailTrap ho?c Ethereal)

Thay ??i EmailSettings trong appsettings.Development.json:

```json
"EmailSettings": {
  "SmtpHost": "smtp.ethereal.email",
  "SmtpPort": 587,
  "SmtpUsername": "your-ethereal-username",
  "SmtpPassword": "your-ethereal-password",
  "FromEmail": "noreply@healthcareblog.com",
  "FromName": "HealthCareBlog"
}
```

## Security Notes

1. **Secret Key**: Thay ??i `JwtSettings:SecretKey` trong production
2. **Email Password**: Không commit password th?t vào git
3. **HTTPS**: B?t bu?c s? d?ng HTTPS trong production
4. **CORS**: Ch? cho phép domain frontend th?c t? trong production

## TODO List

- [ ] Thay ??i `FrontendUrl` trong appsettings.json khi deploy
- [ ] C?u hình SMTP credentials th?c t?
- [ ] T?o frontend routes: `/auth/verify-email`, `/auth/reset-password`, `/auth/forgot-password`
- [ ] Implement password strength indicator
- [ ] Add rate limiting cho authentication endpoints
- [ ] Setup email templates v?i logo th?c t?
