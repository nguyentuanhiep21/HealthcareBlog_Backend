using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareBlog_Backend.Models.Entities;

/// <summary>
/// Cuộc hội thoại 1-1 giữa 2 người dùng.
/// user1_id luôn < user2_id (normalized) để tránh duplicate.
/// </summary>
[Table("conversations")]
public class Conversation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user1_id")]
    [StringLength(450)]
    public string User1Id { get; set; } = string.Empty;

    [Column("user2_id")]
    [StringLength(450)]
    public string User2Id { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_message_at")]
    public DateTime? LastMessageAt { get; set; }

    [Column("last_message_preview")]
    [StringLength(200)]
    public string? LastMessagePreview { get; set; }

    // ===== Navigation =====
    [ForeignKey(nameof(User1Id))]
    public virtual User? User1 { get; set; }

    [ForeignKey(nameof(User2Id))]
    public virtual User? User2 { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
