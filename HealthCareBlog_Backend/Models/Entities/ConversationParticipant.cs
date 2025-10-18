using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng ConversationParticipants - Quản lý thành viên trong cuộc hội thoại
/// Kết nối giữa User và Conversation (many-to-many relationship)
/// Lưu trữ thời gian tham gia và thời gian đọc tin nhắn cuối
/// Dùng để hiển thị danh sách cuộc trò chuyện và đếm tin nhắn chưa đọc
/// </summary>
[Table("conversation_participants")]
public partial class ConversationParticipant
{
    [Key]
    [Column("participant_id")]
    public int ParticipantId { get; set; } // ID thành viên

    [Required]
    [Column("conversation_id")]
    public int ConversationId { get; set; } // ID cuộc hội thoại

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!; // ID người tham gia

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow; // Thời gian tham gia

    [Column("last_read_at")]
    public DateTime? LastReadAt { get; set; } // Thời gian đọc tin nhắn cuối (để đếm unread)

    // Navigation Properties
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
