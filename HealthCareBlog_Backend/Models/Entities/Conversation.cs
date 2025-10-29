using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Conversations - Quản lý cuộc trò chuyện giữa người dùng
/// Lưu loại cuộc trò chuyện (Direct/Group), tên, avatar và thông tin thời gian
/// Dùng cho chức năng nhắn tin và lưu lịch sử hội thoại
/// </summary>
[Table("conversations")]
public class Conversation
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID cuộc trò chuyện

    [Column("type")]
    public ConversationType Type { get; set; } // Loại: Direct (1-1) hoặc Group

    [Column("name")]
    [StringLength(200)]
    public string? Name { get; set; } // Tên cuộc trò chuyện (nhóm)

    [Column("avatar_url")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; } // Ảnh đại diện (nhóm)

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian tạo

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật

    // ========== NAVIGATION PROPERTIES ==========
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>(); // Danh sách người tham gia
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>(); // Tin nhắn trong cuộc trò chuyện
}
