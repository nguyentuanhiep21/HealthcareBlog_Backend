using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng ConversationParticipants - Liệt kê người tham gia trong một cuộc trò chuyện
/// Lưu thời gian tham gia/rời và liên kết tới user và conversation
/// Dùng để quản lý thành viên trong cuộc trò chuyện và phân quyền truy cập tin nhắn
/// </summary>
[Table("conversation_participants")]
public class ConversationParticipant
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID của bản ghi tham gia

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } // Thời gian tham gia

    [Column("left_at")]
    public DateTime? LeftAt { get; set; } // Thời gian rời (nếu có)

    // ========== FOREIGN KEYS ==========
    [Column("conversation_id")]
    [Required]
    public int ConversationId { get; set; } // ID cuộc trò chuyện

    [Column("user_id")]
    [StringLength(450)]
    [Required]
    public string UserId { get; set; } = string.Empty; // ID người tham gia

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!; // Cuộc trò chuyện

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!; // Người dùng (navigation)
}
