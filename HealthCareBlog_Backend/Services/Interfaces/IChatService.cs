using HealthCareBlog_Backend.Models.DTOs.Chat;

namespace HealthCareBlog_Backend.Services.Interfaces;

public interface IChatService
{
    /// <summary>Lấy danh sách cuộc hội thoại của user hiện tại</summary>
    Task<List<ConversationDTO>> GetConversationsAsync(string userId, int page, int pageSize);

    /// <summary>Lấy hoặc tạo mới cuộc hội thoại với targetUserId</summary>
    Task<ConversationDTO?> GetOrCreateConversationAsync(string currentUserId, string targetUserId);

    /// <summary>
    /// Lấy lịch sử tin nhắn — chỉ thành viên của conversation mới được xem.
    /// Trả về null nếu không có quyền.
    /// </summary>
    Task<List<MessageDTO>?> GetMessagesAsync(string currentUserId, int conversationId, int page, int pageSize);

    /// <summary>Gửi tin nhắn, trả về MessageDTO để broadcast qua SignalR</summary>
    Task<MessageDTO?> SendMessageAsync(string senderId, int conversationId, string content);

    /// <summary>Đánh dấu tất cả tin trong conversation là đã đọc</summary>
    Task<bool> MarkMessagesAsReadAsync(string currentUserId, int conversationId);

    /// <summary>Tổng số tin chưa đọc (badge trên icon chat)</summary>
    Task<int> GetTotalUnreadCountAsync(string userId);
}
