using HealthCareBlog_Backend.Models.Entities;

namespace HealthCareBlog_Backend.Interfaces;

public interface IChatRepository
{
    /// <summary>Lấy danh sách conversations của user, sắp xếp theo last_message_at</summary>
    Task<List<Conversation>> GetConversationsAsync(string userId, int page, int pageSize);

    /// <summary>Lấy hoặc tạo mới conversation giữa 2 user</summary>
    Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id);

    /// <summary>Lấy conversation theo Id</summary>
    Task<Conversation?> GetConversationByIdAsync(int conversationId);

    /// <summary>Lấy lịch sử tin nhắn của conversation (phân trang, mới nhất trước)</summary>
    Task<List<Message>> GetMessagesAsync(int conversationId, int page, int pageSize);

    /// <summary>Thêm tin nhắn mới</summary>
    Task<Message> AddMessageAsync(Message message);

    /// <summary>Cập nhật LastMessageAt và LastMessagePreview trên conversation</summary>
    Task UpdateConversationPreviewAsync(int conversationId, string preview, DateTime sentAt);

    /// <summary>Đánh dấu tất cả tin nhắn chưa đọc (gửi bởi người kia) là đã đọc</summary>
    Task MarkMessagesAsReadAsync(int conversationId, string readerId);

    /// <summary>Đếm tổng số tin chưa đọc của user (trên tất cả conversations)</summary>
    Task<int> GetTotalUnreadCountAsync(string userId);

    /// <summary>Đếm số tin chưa đọc trong một conversation cụ thể</summary>
    Task<int> GetUnreadCountInConversationAsync(int conversationId, string userId);

    Task<int> SaveChangesAsync();
}
