using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Chat;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HealthCareBlog_Backend.Hubs;

/// <summary>
/// SignalR Hub cho tính năng chat realtime.
///
/// Client kết nối tới: /hubs/chat
///
/// Client → Server methods:
///   - SendMessage(conversationId, content)
///   - MarkAsRead(conversationId)
///   - JoinConversation(conversationId)
///   - LeaveConversation(conversationId)
///
/// Server → Client events:
///   - ReceiveMessage(MessageDTO)
///   - MessagesRead(conversationId, readerId)
///   - Error(message)
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IPresenceTracker _tracker;

    public ChatHub(IChatService chatService, IPresenceTracker tracker)
    {
        _chatService = chatService;
        _tracker = tracker;
    }

    // ===== Client → Server =====

    /// <summary>
    /// Client gửi tin nhắn.
    /// Hub lưu vào DB, sau đó push ReceiveMessage cho tất cả thành viên của conversation.
    /// </summary>
    public async Task SendMessage(int conversationId, string content)
    {
        var senderId = Context.User?.GetUserId();
        if (senderId is null)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized");
            return;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            await Clients.Caller.SendAsync("Error", "Message content cannot be empty");
            return;
        }

        var messageDto = await _chatService.SendMessageAsync(senderId, conversationId, content);
        if (messageDto is null)
        {
            await Clients.Caller.SendAsync("Error", "Cannot send message — conversation not found or access denied");
            return;
        }

        // Broadcast cho toàn bộ group của conversation (cả sender lẫn receiver)
        await Clients.Group(GetConversationGroup(conversationId))
            .SendAsync("ReceiveMessage", messageDto);
    }

    /// <summary>
    /// Client join vào SignalR group của một conversation để nhận tin realtime.
    /// Gọi ngay sau khi mở cửa sổ chat.
    /// </summary>
    public async Task JoinConversation(int conversationId)
    {
        var userId = Context.User?.GetUserId();
        if (userId is null) return;

        // Chỉ cho phép join nếu user là thành viên
        var messages = await _chatService.GetMessagesAsync(userId, conversationId, 1, 1);
        if (messages is null)
        {
            await Clients.Caller.SendAsync("Error", "Access denied to conversation");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
    }

    /// <summary>
    /// Client leave group (đóng cửa sổ chat).
    /// </summary>
    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
    }

    /// <summary>
    /// Client báo đã đọc tin nhắn trong conversation.
    /// Push sự kiện MessagesRead để sender biết tin đã được đọc.
    /// </summary>
    public async Task MarkAsRead(int conversationId)
    {
        var userId = Context.User?.GetUserId();
        if (userId is null) return;

        var success = await _chatService.MarkMessagesAsReadAsync(userId, conversationId);
        if (success)
        {
            // Thông báo cho group rằng userId đã đọc tin
            await Clients.Group(GetConversationGroup(conversationId))
                .SendAsync("MessagesRead", conversationId, userId);
        }
    }

    // ===== Lifecycle =====

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetUserId();
        if (userId is not null)
        {
            // Mỗi user join group riêng để nhận notification toàn cục (unread count, v.v.)
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroup(userId));
            
            // Cập nhật trạng thái online
            var isFirstConnection = await _tracker.UserConnected(userId, Context.ConnectionId);
            if (isFirstConnection)
            {
                // Báo cho tất cả client biết user này vừa online
                await Clients.All.SendAsync("UserIsOnline", userId);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.GetUserId();
        if (userId is not null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroup(userId));
            
            // Cập nhật trạng thái offline
            var isLastConnection = await _tracker.UserDisconnected(userId, Context.ConnectionId);
            if (isLastConnection)
            {
                // Báo cho tất cả client biết user này vừa offline
                await Clients.All.SendAsync("UserIsOffline", userId);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ===== Group name helpers =====
    private static string GetConversationGroup(int conversationId) => $"conversation_{conversationId}";
    private static string GetUserGroup(string userId) => $"user_{userId}";
}
