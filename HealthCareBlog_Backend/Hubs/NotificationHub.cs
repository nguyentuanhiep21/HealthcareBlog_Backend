using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HealthCareBlog_Backend.Hubs;

/// <summary>
/// SignalR Hub cho tính năng thông báo (Notification).
/// Client kết nối tới: /hubs/notification
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    // Hub này chủ yếu để server push notification xuống client.
    // Client chỉ cần kết nối và lắng nghe sự kiện "ReceiveNotification".
}
