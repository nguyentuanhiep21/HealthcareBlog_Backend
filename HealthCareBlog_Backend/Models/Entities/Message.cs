using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Bảng Messages - Lưu trữ tin nhắn trong các cuộc trò chuyện
/// Hỗ trợ nội dung text và hình ảnh, cùng thông tin xóa/metadata
/// </summary>
[Table("messages")]
public class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // ID tin nhắn

    [Column("content")]
    [StringLength(5000)]
    public string? Content { get; set; } // Nội dung văn bản của tin nhắn

    [Column("image_url")]
    [StringLength(500)]
    public string? ImageUrl { get; set; } // URL hình ảnh kèm theo (nếu có)

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } // Cờ xóa mềm

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } // Thời gian gửi

    // ========== FOREIGN KEYS ==========
    [Column("conversation_id")]
    [Required]
    public int ConversationId { get; set; } // ID cuộc trò chuyện

    [Column("sender_id")]
    [StringLength(450)]
    [Required]
    public string SenderId { get; set; } = string.Empty; // ID người gửi

    // ========== NAVIGATION PROPERTIES ==========
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!; // Cuộc trò chuyện chứa tin nhắn

    [ForeignKey("SenderId")]
    public virtual ApplicationUser Sender { get; set; } = null!; // Người gửi (navigation)
}
