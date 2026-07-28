using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Conversation>> GetConversationsAsync(string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        return await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id)
    {
        // Normalize: luôn lưu user nhỏ hơn vào user1_id để đảm bảo unique
        var (normalizedUser1, normalizedUser2) = string.Compare(user1Id, user2Id, StringComparison.Ordinal) < 0
            ? (user1Id, user2Id)
            : (user2Id, user1Id);

        var existing = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.User1Id == normalizedUser1 && c.User2Id == normalizedUser2);

        if (existing is not null)
            return existing;

        var conversation = new Conversation
        {
            User1Id = normalizedUser1,
            User2Id = normalizedUser2,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        // Reload với navigation properties
        return await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstAsync(c => c.Id == conversation.Id);
    }

    public async Task<Conversation?> GetConversationByIdAsync(int conversationId)
        => await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == conversationId);

    public async Task<List<Message>> GetMessagesAsync(int conversationId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 30;
        if (pageSize > 100) pageSize = 100;

        return await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task UpdateConversationPreviewAsync(int conversationId, string preview, DateTime sentAt)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation is null) return;

        conversation.LastMessagePreview = preview.Length > 200 ? preview[..200] : preview;
        conversation.LastMessageAt = sentAt;
        await _context.SaveChangesAsync();
    }

    public async Task MarkMessagesAsReadAsync(int conversationId, string readerId)
    {
        // Đánh dấu đã đọc những tin nhắn gửi bởi người kia (không phải readerId)
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId
                        && m.SenderId != readerId
                        && !m.IsRead)
            .ToListAsync();

        if (unreadMessages.Count == 0) return;

        foreach (var msg in unreadMessages)
            msg.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalUnreadCountAsync(string userId)
    {
        // Conversations mà userId tham gia
        var conversationIds = await _context.Conversations
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .Select(c => c.Id)
            .ToListAsync();

        return await _context.Messages
            .CountAsync(m => conversationIds.Contains(m.ConversationId)
                             && m.SenderId != userId
                             && !m.IsRead);
    }

    public async Task<int> GetUnreadCountInConversationAsync(int conversationId, string userId)
        => await _context.Messages
            .CountAsync(m => m.ConversationId == conversationId
                             && m.SenderId != userId
                             && !m.IsRead);

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
