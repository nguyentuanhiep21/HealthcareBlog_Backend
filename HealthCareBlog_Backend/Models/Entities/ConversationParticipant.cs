using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("conversation_participants")]
public partial class ConversationParticipant
{
    [Key]
    [Column("participant_id")]
    public int ParticipantId { get; set; }

    [Required]
    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Required]
    [Column("user_id")]
    public string UserId { get; set; } = null!;

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Column("last_read_at")]
    public DateTime? LastReadAt { get; set; }

    // Navigation Properties
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
