using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("messages")]
public partial class Message
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Required]
    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Required]
    [Column("sender_id")]
    public string SenderId { get; set; } = null!;

    [Required]
    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("media_urls")]
    public string? MediaUrls { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("SenderId")]
    public virtual ApplicationUser Sender { get; set; } = null!;
}
