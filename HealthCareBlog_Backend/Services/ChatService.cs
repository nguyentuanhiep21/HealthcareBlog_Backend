using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Models.DTOs.Chat;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<List<ConversationDTO>> GetConversationsAsync(string userId, int page, int pageSize)
    {
        var conversations = await _chatRepository.GetConversationsAsync(userId, page, pageSize);

        var result = new List<ConversationDTO>();
        foreach (var conv in conversations)
        {
            var unread = await _chatRepository.GetUnreadCountInConversationAsync(conv.Id, userId);
            result.Add(MapToConversationDTO(conv, userId, unread));
        }
        return result;
    }

    public async Task<ConversationDTO?> GetOrCreateConversationAsync(string currentUserId, string targetUserId)
    {
        if (currentUserId == targetUserId)
            return null; // Không thể chat với chính mình

        var conversation = await _chatRepository.GetOrCreateConversationAsync(currentUserId, targetUserId);
        var unread = await _chatRepository.GetUnreadCountInConversationAsync(conversation.Id, currentUserId);
        return MapToConversationDTO(conversation, currentUserId, unread);
    }

    public async Task<List<MessageDTO>?> GetMessagesAsync(string currentUserId, int conversationId, int page, int pageSize)
    {
        var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
        if (conversation is null) return null;

        // Kiểm tra quyền: chỉ thành viên mới được xem
        if (!IsMember(conversation, currentUserId)) return null;

        var messages = await _chatRepository.GetMessagesAsync(conversationId, page, pageSize);
        return messages.Select(MapToMessageDTO).ToList();
    }

    public async Task<MessageDTO?> SendMessageAsync(string senderId, int conversationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
        if (conversation is null) return null;

        // Kiểm tra quyền
        if (!IsMember(conversation, senderId)) return null;

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _chatRepository.AddMessageAsync(message);

        // Cập nhật preview trên conversation
        await _chatRepository.UpdateConversationPreviewAsync(conversationId, content.Trim(), saved.CreatedAt);

        // Reload với sender info để trả về DTO đầy đủ
        var fullConversation = await _chatRepository.GetConversationByIdAsync(conversationId);
        var sender = senderId == fullConversation?.User1Id ? fullConversation.User1 : fullConversation?.User2;

        return new MessageDTO
        {
            Id = saved.Id,
            ConversationId = saved.ConversationId,
            SenderId = saved.SenderId,
            SenderName = sender?.FullName ?? sender?.UserName ?? "Unknown",
            SenderAvatarUrl = sender?.AvatarUrl,
            Content = saved.Content,
            IsRead = saved.IsRead,
            CreatedAt = saved.CreatedAt
        };
    }

    public async Task<bool> MarkMessagesAsReadAsync(string currentUserId, int conversationId)
    {
        var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
        if (conversation is null) return false;
        if (!IsMember(conversation, currentUserId)) return false;

        await _chatRepository.MarkMessagesAsReadAsync(conversationId, currentUserId);
        return true;
    }

    public Task<int> GetTotalUnreadCountAsync(string userId)
        => _chatRepository.GetTotalUnreadCountAsync(userId);

    // ===== Helpers =====

    private static bool IsMember(Conversation conversation, string userId)
        => conversation.User1Id == userId || conversation.User2Id == userId;

    private static ConversationDTO MapToConversationDTO(Conversation conv, string currentUserId, int unreadCount)
    {
        // Lấy thông tin người kia
        var isUser1 = conv.User1Id == currentUserId;
        var otherUser = isUser1 ? conv.User2 : conv.User1;

        return new ConversationDTO
        {
            Id = conv.Id,
            OtherUser = new ChatUserDTO
            {
                Id = otherUser?.Id ?? string.Empty,
                FullName = otherUser?.FullName ?? otherUser?.UserName ?? "Unknown",
                AvatarUrl = otherUser?.AvatarUrl,
                UserName = otherUser?.UserName
            },
            LastMessagePreview = conv.LastMessagePreview,
            LastMessageAt = conv.LastMessageAt,
            UnreadCount = unreadCount,
            CreatedAt = conv.CreatedAt
        };
    }

    private static MessageDTO MapToMessageDTO(Message m) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        SenderName = m.Sender?.FullName ?? m.Sender?.UserName ?? "Unknown",
        SenderAvatarUrl = m.Sender?.AvatarUrl,
        Content = m.Content,
        IsRead = m.IsRead,
        CreatedAt = m.CreatedAt
    };
}
