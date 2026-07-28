using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Tin nhắn trong một cuộc hội thoại.
/// </summary>
[Table("messages")]
public class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Column("sender_id")]
    [StringLength(450)]
    public string SenderId { get; set; } = string.Empty;

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ===== Navigation =====
    [ForeignKey(nameof(ConversationId))]
    public virtual Conversation? Conversation { get; set; }

    [ForeignKey(nameof(SenderId))]
    public virtual User? Sender { get; set; }
}
