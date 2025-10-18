using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Messages - Quản lý tin nhắn trong cuộc hội thoại
/// Lưu trữ nội dung tin nhắn, file đính kèm, trạng thái đã đọc
/// Hỗ trợ gửi text và media (ảnh, video, file)
/// Theo dõi thời gian gửi và chỉnh sửa tin nhắn
/// </summary>
[Table("messages")]
public partial class Message
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; } // ID tin nhắn

    [Required]
    [Column("conversation_id")]
    public int ConversationId { get; set; } // ID cuộc hội thoại

    [Required]
    [Column("sender_id")]
    public string SenderId { get; set; } = null!; // ID người gửi

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!; // Nội dung tin nhắn

    [Column("media_urls")]
    public string? MediaUrls { get; set; } // JSON array chứa URL file đính kèm

    [Column("is_read")]
    public bool IsRead { get; set; } = false; // Trạng thái đã đọc

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian gửi

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // Thời gian chỉnh sửa

    // Navigation Properties
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("SenderId")]
    public virtual ApplicationUser Sender { get; set; } = null!;
}
