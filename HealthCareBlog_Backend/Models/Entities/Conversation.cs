using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Conversations - Quản lý cuộc hội thoại
/// Lưu trữ thông tin về cuộc trò chuyện giữa các user
/// Hỗ trợ cả chat 1-1 (OneToOne) và chat nhóm (Group)
/// Theo dõi thời gian tin nhắn cuối để sắp xếp danh sách chat
/// </summary>
[Table("conversations")]
public partial class Conversation
{
    [Key]
    [Column("conversation_id")]
    public int ConversationId { get; set; } // ID cuộc hội thoại

    [Required]
    [Column("conversation_type")]
    [StringLength(20)]
    public string ConversationType { get; set; } = "OneToOne"; // Loại: OneToOne hoặc Group

    [Column("name")]
    [StringLength(100)]
    public string? Name { get; set; } // Tên nhóm (null nếu là chat 1-1)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo cuộc hội thoại

    [Column("last_message_at")]
    public DateTime? LastMessageAt { get; set; } // Thời gian tin nhắn cuối (để sắp xếp)

    // Navigation Properties
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
