using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace HealthCareBlog_Backend.Models.Entities;

[Table("users")]
public partial class ApplicationUser : IdentityUser
{
    [Column("user_id")]
    public override string Id { get; set; } = null!;

    [Column("user_name")]
    public override string? UserName { get; set; }

    [Column("email")]
    public override string? Email { get; set; }

    [Column("phone_number")]
    public override string? PhoneNumber { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Column("bio")]
    [StringLength(500)]
    public string? Bio { get; set; }

    [Column("profile_picture")]
    public string? ProfilePicture { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("height")]
    public decimal? Height { get; set; }

    [Column("weight")]
    public decimal? Weight { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
    public virtual HealthProfile? HealthProfile { get; set; }
    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
    public virtual ICollection<ReportedContent> Reports { get; set; } = new List<ReportedContent>();
    public virtual ICollection<MealSuggestion> MealSuggestions { get; set; } = new List<MealSuggestion>();
}
