using HealthCareBlog_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HealthCareBlog_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<HealthProfile> HealthProfiles => Set<HealthProfile>();
        public DbSet<MealSuggestion> MealSuggestions => Set<MealSuggestion>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ReportedContent> ReportedContents => Set<ReportedContent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            // Configure Follow
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.FollowingUser)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            });

            // Configure Post
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Like
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(c => c.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Conversation
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasIndex(e => e.LastMessageAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure ConversationParticipant
            modelBuilder.Entity<ConversationParticipant>(entity =>
            {
                entity.HasOne(cp => cp.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cp => cp.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                    .WithMany(u => u.ConversationParticipants)
                    .HasForeignKey(cp => cp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ConversationId, e.UserId }).IsUnique();

                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure HealthProfile
            modelBuilder.Entity<HealthProfile>(entity =>
            {
                entity.HasOne(hp => hp.User)
                    .WithOne(u => u.HealthProfile)
                    .HasForeignKey<HealthProfile>(hp => hp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure MealSuggestion
            modelBuilder.Entity<MealSuggestion>(entity =>
            {
                entity.HasOne(ms => ms.User)
                    .WithMany(u => u.MealSuggestions)
                    .HasForeignKey(ms => ms.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.ReceivedNotifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(al => al.Admin)
                    .WithMany()
                    .HasForeignKey(al => al.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure ReportedContent
            modelBuilder.Entity<ReportedContent>(entity =>
            {
                entity.HasOne(rc => rc.Reporter)
                    .WithMany(u => u.Reports)
                    .HasForeignKey(rc => rc.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultSQLConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultSQLConnection' not found.");

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
