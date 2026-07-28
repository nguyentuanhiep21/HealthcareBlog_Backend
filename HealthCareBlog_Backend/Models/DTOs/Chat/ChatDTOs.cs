namespace HealthCareBlog_Backend.Models.DTOs.Chat;

/// <summary>
/// Thông tin tóm tắt của một cuộc hội thoại (dùng trong danh sách chat).
/// </summary>
public class ConversationDTO
{
    public int Id { get; set; }
    /// <summary>Thông tin của người dùng đối diện (không phải current user)</summary>
    public ChatUserDTO OtherUser { get; set; } = null!;
    public string? LastMessagePreview { get; set; }
    public DateTime? LastMessageAt { get; set; }
    /// <summary>Số tin nhắn chưa đọc trong cuộc hội thoại này</summary>
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Thông tin người dùng đối diện trong cuộc hội thoại.
/// </summary>
public class ChatUserDTO
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? UserName { get; set; }
}

/// <summary>
/// Tin nhắn trong một cuộc hội thoại.
/// </summary>
public class MessageDTO
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string? SenderAvatarUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request body để gửi một tin nhắn mới.
/// </summary>
public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
}
