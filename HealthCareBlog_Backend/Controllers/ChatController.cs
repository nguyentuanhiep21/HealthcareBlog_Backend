using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Chat;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

/// <summary>
/// REST API cho tính năng chat.
/// Dùng để lấy lịch sử, danh sách conversations, unread count.
/// Gửi tin nhắn thời gian thực qua SignalR Hub (/hubs/chat).
/// </summary>
[Route("api/chats")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// GET api/chat/conversations
    /// Danh sách cuộc hội thoại của user hiện tại, sắp xếp theo tin nhắn mới nhất.
    /// </summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDTO>>> GetConversations(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId()!;
        var conversations = await _chatService.GetConversationsAsync(userId, page, pageSize);
        return Ok(conversations);
    }

    /// <summary>
    /// POST api/chat/conversations/{targetUserId}
    /// Lấy hoặc tạo mới cuộc hội thoại với người dùng có Id = targetUserId.
    /// Frontend gọi endpoint này khi user bấm nút "Nhắn tin" trên profile.
    /// </summary>
    [HttpPost("conversations/{targetUserId}")]
    public async Task<ActionResult<ConversationDTO>> GetOrCreateConversation(string targetUserId)
    {
        var currentUserId = User.GetUserId()!;

        if (currentUserId == targetUserId)
            return BadRequest(new { message = "Không thể tự nhắn tin với chính mình." });

        var conversation = await _chatService.GetOrCreateConversationAsync(currentUserId, targetUserId);
        if (conversation is null)
            return BadRequest(new { message = "Không thể tạo cuộc hội thoại." });

        return Ok(conversation);
    }

    /// <summary>
    /// GET api/chat/conversations/{conversationId}/messages
    /// Lịch sử tin nhắn trong conversation (phân trang, mới nhất trước).
    /// </summary>
    [HttpGet("conversations/{conversationId:int}/messages")]
    public async Task<ActionResult<List<MessageDTO>>> GetMessages(
        int conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var userId = User.GetUserId()!;
        var messages = await _chatService.GetMessagesAsync(userId, conversationId, page, pageSize);

        if (messages is null)
            return Forbid(); // User không phải thành viên của conversation

        return Ok(messages);
    }

    /// <summary>
    /// PUT api/chat/conversations/{conversationId}/read
    /// Đánh dấu tất cả tin nhắn trong conversation là đã đọc.
    /// Cũng có thể gọi qua SignalR Hub (MarkAsRead method).
    /// </summary>
    [HttpPut("conversations/{conversationId:int}/read")]
    public async Task<ActionResult> MarkAsRead(int conversationId)
    {
        var userId = User.GetUserId()!;
        var success = await _chatService.MarkMessagesAsReadAsync(userId, conversationId);

        if (!success)
            return NotFound(new { message = "Conversation không tồn tại hoặc bạn không có quyền truy cập." });

        return Ok(new { message = "Đã đánh dấu tất cả tin nhắn là đã đọc." });
    }

    /// <summary>
    /// GET api/chat/unread-count
    /// Tổng số tin nhắn chưa đọc (dùng hiển thị badge trên icon chat).
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = User.GetUserId()!;
        var count = await _chatService.GetTotalUnreadCountAsync(userId);
        return Ok(new { count });
    }
}
