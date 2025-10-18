using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("conversations")]
public partial class Conversation
{
    [Key]
    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Required]
    [Column("conversation_type")]
    [StringLength(20)]
    public string ConversationType { get; set; } = "OneToOne";

    [Column("name")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_message_at")]
    public DateTime? LastMessageAt { get; set; }

    // Navigation Properties
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
