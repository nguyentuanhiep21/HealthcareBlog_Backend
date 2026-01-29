# 🔐 Backend Implementation: Khóa Tài Khoản Có Thời Hạn

## 📋 Công Việc Cần Làm

Cập nhật logic backend để hỗ trợ khóa tài khoản có thời hạn (unlock date).

---

## 1️⃣ Tạo DTO Request

### File: `Models/DTOs/LockUserRequest.cs` (NEW)

```csharp
namespace HealthCareBlog_Backend.Models.DTOs;

public class LockUserRequest
{
    /// <summary>
    /// Ngày mở khóa tự động (null = khóa vĩnh viễn)
    /// </summary>
    public DateTime? UnlockDate { get; set; }

    /// <summary>
    /// Lý do khóa (null = mở khóa)
    /// </summary>
    public string? Reason { get; set; }
}
```

---

## 2️⃣ Cập Nhật Controller

### File: `Controllers/UserController.cs`

**Tìm method `ToggleLock` và cập nhật:**

```csharp
[HttpPut("{id}/toggle-lock")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> ToggleLock(string id, [FromBody] LockUserRequest request)
{
    try
    {
        // Kiểm tra admin
        var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized("User ID not found in token");

        // Kiểm tra user tồn tại
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found");

        // Kiểm tra không khóa admin
        var adminRole = await _userManager.IsInRoleAsync(user, "Admin");
        if (adminRole)
            return BadRequest("Cannot lock admin accounts");

        // Validate request
        if (request.UnlockDate.HasValue)
        {
            // Khóa có thời hạn
            var unlockDate = request.UnlockDate.Value;
            var now = DateTime.UtcNow;

            // Validate: ngày mở khóa phải > hôm nay
            if (unlockDate <= now)
                return BadRequest("Unlock date must be in the future");

            // Validate: không quá 1 năm
            var maxDate = now.AddYears(1);
            if (unlockDate > maxDate)
                return BadRequest("Unlock date cannot exceed 1 year from now");

            // Cập nhật user
            user.IsLocked = true;
            user.UnlockDate = unlockDate;
            user.LockedAt = now;
            user.LockReason = request.Reason ?? $"Locked until {unlockDate:yyyy-MM-dd}";
        }
        else if (request.Reason != null)
        {
            // Khóa vĩnh viễn
            user.IsLocked = true;
            user.UnlockDate = null;
            user.LockedAt = DateTime.UtcNow;
            user.LockReason = request.Reason;
        }
        else
        {
            // Mở khóa
            user.IsLocked = false;
            user.UnlockDate = null;
            user.LockedAt = null;
            user.LockReason = null;
        }

        // Lưu vào database
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        // Log action
        await _logger.LogAsync(new ActivityLog
        {
            UserId = adminId,
            Action = user.IsLocked ? "LOCK_ACCOUNT" : "UNLOCK_ACCOUNT",
            TargetUserId = id,
            Description = user.IsLocked 
                ? $"Locked until {user.UnlockDate:yyyy-MM-dd}"
                : "Account unlocked",
            CreatedAt = DateTime.UtcNow
        });

        return Ok(new
        {
            message = user.IsLocked ? "Account locked successfully" : "Account unlocked successfully",
            userId = id,
            isLocked = user.IsLocked,
            unlockDate = user.UnlockDate,
            reason = user.LockReason
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error toggling user lock status");
        return StatusCode(500, "An error occurred while processing the request");
    }
}
```

---

## 3️⃣ Service - Auto Unlock (Tuỳ Chọn)

### File: `Services/AutoUnlockUserService.cs` (NEW)

**Mục đích:** Tự động mở khóa tài khoản khi hết hạn

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Data;

namespace HealthCareBlog_Backend.Services;

public interface IAutoUnlockUserService
{
    Task ProcessExpiredLocksAsync();
}

public class AutoUnlockUserService : IAutoUnlockUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDBContext _context;
    private readonly ILogger<AutoUnlockUserService> _logger;

    public AutoUnlockUserService(
        UserManager<User> userManager,
        ApplicationDBContext context,
        ILogger<AutoUnlockUserService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task ProcessExpiredLocksAsync()
    {
        try
        {
            var now = DateTime.UtcNow;

            // Lấy tất cả user bị khóa có unlockDate <= now
            var expiredLockedUsers = await _context.Users
                .Where(u => u.IsLocked && u.UnlockDate != null && u.UnlockDate <= now)
                .ToListAsync();

            if (expiredLockedUsers.Count == 0)
            {
                _logger.LogInformation("No expired locks found");
                return;
            }

            foreach (var user in expiredLockedUsers)
            {
                user.IsLocked = false;
                user.UnlockDate = null;
                user.LockedAt = null;
                user.LockReason = "Auto-unlocked - Lock period expired";

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Auto-unlocked user {user.UserName} (ID: {user.Id})");
                }
                else
                {
                    _logger.LogError($"Failed to auto-unlock user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired locks");
        }
    }
}

public class AutoUnlockUserBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutoUnlockUserBackgroundService> _logger;

    public AutoUnlockUserBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AutoUnlockUserBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AutoUnlockUserBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IAutoUnlockUserService>();
                    await service.ProcessExpiredLocksAsync();
                }

                // Chạy mỗi 1 giờ
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AutoUnlockUserBackgroundService");
            }
        }

        _logger.LogInformation("AutoUnlockUserBackgroundService stopped");
    }
}
```

---

## 4️⃣ Đăng Ký Service trong Program.cs

```csharp
// Program.cs

// Add auto unlock service
builder.Services.AddScoped<IAutoUnlockUserService, AutoUnlockUserService>();
builder.Services.AddHostedService<AutoUnlockUserBackgroundService>();
```

---

## 5️⃣ Tạo Migration

```bash
cd HealthCareBlog_Backend/HealthCareBlog_Backend

# Xóa migration cũ nếu chưa apply
dotnet ef migrations remove

# Tạo migration mới
dotnet ef migrations add "add_unlock_date_to_user"

# Apply lên database
dotnet ef database update
```

**Migration sẽ tạo:**
- Cột `unlock_date` (DateTime?, nullable)
- Index trên `is_locked` để query nhanh

---

## 6️⃣ Cập Nhật GetUsers Admin Endpoint

**File:** `Controllers/UserController.cs`

Kiểm tra `GetAllUsers` endpoint (admin view) để chắc chắn trả về `unlockDate`:

```csharp
[HttpGet("admin/all")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllUsers(
    string? searchQuery,
    int page = 1,
    int pageSize = 20)
{
    var query = _context.Users.AsQueryable();

    if (!string.IsNullOrEmpty(searchQuery))
    {
        query = query.Where(u =>
            u.UserName!.Contains(searchQuery) ||
            u.Email!.Contains(searchQuery) ||
            (u.FullName != null && u.FullName.Contains(searchQuery)));
    }

    var users = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new AdminUserDto
        {
            Id = u.Id,
            Username = u.UserName ?? "",
            Email = u.Email ?? "",
            FullName = u.FullName,
            Bio = u.Bio,
            AvatarUrl = u.AvatarUrl,
            FollowersCount = u.FollowerCount,
            FollowingCount = u.FollowingCount,
            PostsCount = u.PostCount,
            IsLocked = u.IsLocked,
            CreatedAt = u.UserName ?? "",  // Adjust as needed
            LockedAt = u.LockedAt.HasValue ? u.LockedAt.Value.ToString("o") : null,
            UnlockDate = u.UnlockDate.HasValue ? u.UnlockDate.Value.ToString("o") : null  // ← ADD THIS
        })
        .ToListAsync();

    return Ok(users);
}
```

**Ensure AdminUserDto có field:**
```csharp
public string? UnlockDate { get; set; }
```

---

## 🧪 Testing API

### Test 1: Khóa tài khoản có thời hạn

```bash
curl -X PUT "https://localhost:7223/api/User/{userId}/toggle-lock" \
  -H "Authorization: Bearer {adminToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "unlockDate": "2026-02-20T00:00:00Z",
    "reason": "Khóa vì vi phạm điều khoản"
  }'
```

**Expected Response:**
```json
{
  "message": "Account locked successfully",
  "userId": "...",
  "isLocked": true,
  "unlockDate": "2026-02-20T00:00:00Z",
  "reason": "Khóa vì vi phạm điều khoản"
}
```

### Test 2: Khóa vĩnh viễn

```bash
curl -X PUT "https://localhost:7223/api/User/{userId}/toggle-lock" \
  -H "Authorization: Bearer {adminToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "unlockDate": null,
    "reason": "Khóa vĩnh viễn - Vi phạm nghiêm trọng"
  }'
```

### Test 3: Mở khóa

```bash
curl -X PUT "https://localhost:7223/api/User/{userId}/toggle-lock" \
  -H "Authorization: Bearer {adminToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "unlockDate": null,
    "reason": null
  }'
```

---

## ✅ Checklist

- [ ] Tạo `LockUserRequest.cs` DTO
- [ ] Cập nhật `ToggleLock` method trong UserController
- [ ] (Optional) Tạo AutoUnlockUserService
- [ ] (Optional) Đăng ký service trong Program.cs
- [ ] Tạo migration: `add_unlock_date_to_user`
- [ ] Apply migration lên database
- [ ] Cập nhật `AdminUserDto` có field `unlockDate`
- [ ] Cập nhật `GetAllUsers` endpoint
- [ ] Test API bằng Postman/curl
- [ ] Test frontend integration

---

**Status**: ⏳ Chờ Implementation  
**Difficulty**: 🟢 Dễ  
**Time Estimate**: 30-45 phút
