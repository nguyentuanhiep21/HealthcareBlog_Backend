using HealthCareBlog_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HealthCareBlog_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // ========== DBSETS ==========
        // User & Social
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<UserBlock> UserBlocks => Set<UserBlock>();

        // Post & Content
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostShare> PostShares => Set<PostShare>();
        public DbSet<Hashtag> Hashtags => Set<Hashtag>();
        public DbSet<PostHashtag> PostHashtags => Set<PostHashtag>();

        // Group
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<UserGroup> UserGroups => Set<UserGroup>();
        public DbSet<GroupRole> GroupRoles => Set<GroupRole>();
        public DbSet<GroupJoinRequest> GroupJoinRequests => Set<GroupJoinRequest>();
        public DbSet<GroupPostPending> GroupPostPendings => Set<GroupPostPending>();

        // Messaging
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
        public DbSet<Message> Messages => Set<Message>();

        // Health & AI
        public DbSet<HealthProfile> HealthProfiles => Set<HealthProfile>();
        public DbSet<MealSuggestion> MealSuggestions => Set<MealSuggestion>();

        // System
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ReportedContent> ReportedContents => Set<ReportedContent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== APPLICATION USER CONFIGURATION ==========
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.DeactivatedByAdmin)
                .WithMany()
                .HasForeignKey(u => u.DeactivatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.BannedByAdmin)
                .WithMany()
                .HasForeignKey(u => u.BannedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== FOLLOW CONFIGURATION ==========
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

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== USER BLOCK CONFIGURATION ==========
            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.HasOne(ub => ub.Blocker)
                    .WithMany(u => u.BlockedUsers)
                    .HasForeignKey(ub => ub.BlockerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ub => ub.Blocked)
                    .WithMany(u => u.BlockedByUsers)
                    .HasForeignKey(ub => ub.BlockedId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.BlockerId, e.BlockedId }).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== POST CONFIGURATION ==========
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Group)
                    .WithMany(g => g.Posts)
                    .HasForeignKey(p => p.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.GroupId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.UserId, e.CreatedAt });
                entity.HasIndex(e => new { e.GroupId, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== LIKE CONFIGURATION ==========
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(l => l.CommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.PostId, e.CommentId }).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== COMMENT CONFIGURATION ==========
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

                entity.HasOne(c => c.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ParentCommentId);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== POST SHARE CONFIGURATION ==========
            modelBuilder.Entity<PostShare>(entity =>
            {
                entity.HasOne(ps => ps.Post)
                    .WithMany(p => p.Shares)
                    .HasForeignKey(ps => ps.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ps => ps.User)
                    .WithMany(u => u.PostShares)
                    .HasForeignKey(ps => ps.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ps => ps.Group)
                    .WithMany()
                    .HasForeignKey(ps => ps.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.PostId, e.CreatedAt });
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== HASHTAG CONFIGURATION ==========
            modelBuilder.Entity<Hashtag>(entity =>
            {
                entity.HasIndex(h => h.Name).IsUnique();

                entity.Property(h => h.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== POST HASHTAG CONFIGURATION ==========
            modelBuilder.Entity<PostHashtag>(entity =>
            {
                entity.HasKey(ph => new { ph.PostId, ph.HashtagId });

                entity.HasOne(ph => ph.Post)
                    .WithMany(p => p.PostHashtags)
                    .HasForeignKey(ph => ph.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ph => ph.Hashtag)
                    .WithMany(h => h.PostHashtags)
                    .HasForeignKey(ph => ph.HashtagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== GROUP CONFIGURATION ==========
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasOne(g => g.Owner)
                    .WithMany()
                    .HasForeignKey(g => g.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.DeactivatedByAdmin)
                    .WithMany()
                    .HasForeignKey(g => g.DeactivatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(g => g.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== USER GROUP CONFIGURATION ==========
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasOne(ug => ug.User)
                    .WithMany(u => u.UserGroups)
                    .HasForeignKey(ug => ug.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ug => ug.Group)
                    .WithMany(g => g.UserGroups)
                    .HasForeignKey(ug => ug.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ug => ug.Role)
                    .WithMany(r => r.UserGroups)
                    .HasForeignKey(ug => ug.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.UserId, e.GroupId }).IsUnique();
                entity.HasIndex(e => e.GroupId);

                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== GROUP ROLE CONFIGURATION ==========
            modelBuilder.Entity<GroupRole>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // ========== GROUP JOIN REQUEST CONFIGURATION ==========
            modelBuilder.Entity<GroupJoinRequest>(entity =>
            {
                entity.HasOne(gjr => gjr.Group)
                    .WithMany(g => g.JoinRequests)
                    .HasForeignKey(gjr => gjr.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gjr => gjr.User)
                    .WithMany()
                    .HasForeignKey(gjr => gjr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(gjr => gjr.ResponsedBy)
                    .WithMany()
                    .HasForeignKey(gjr => gjr.ResponsedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.GroupId, e.UserId, e.IsApproved });
                entity.HasIndex(e => e.RequestedAt);

                entity.Property(e => e.RequestedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== GROUP POST PENDING CONFIGURATION ==========
            modelBuilder.Entity<GroupPostPending>(entity =>
            {
                entity.HasOne(gpp => gpp.Group)
                    .WithMany(g => g.PendingPosts)
                    .HasForeignKey(gpp => gpp.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gpp => gpp.Post)
                    .WithMany()
                    .HasForeignKey(gpp => gpp.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(gpp => gpp.ResponsedBy)
                    .WithMany()
                    .HasForeignKey(gpp => gpp.ResponsedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.GroupId, e.IsApproved });
                entity.HasIndex(e => e.SubmittedAt);

                entity.Property(e => e.SubmittedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== CONVERSATION CONFIGURATION ==========
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Type);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== CONVERSATION PARTICIPANT CONFIGURATION ==========
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

            // ========== MESSAGE CONFIGURATION ==========
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
                entity.HasIndex(e => new { e.ConversationId, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== HEALTH PROFILE CONFIGURATION ==========
            modelBuilder.Entity<HealthProfile>(entity =>
            {
                entity.HasOne(hp => hp.User)
                    .WithOne(u => u.HealthProfile)
                    .HasForeignKey<HealthProfile>(hp => hp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== MEAL SUGGESTION CONFIGURATION ==========
            modelBuilder.Entity<MealSuggestion>(entity =>
            {
                entity.HasOne(ms => ms.User)
                    .WithMany(u => u.MealSuggestions)
                    .HasForeignKey(ms => ms.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.UserId, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== NOTIFICATION CONFIGURATION ==========
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.ReceivedNotifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Actor)
                    .WithMany()
                    .HasForeignKey(n => n.ActorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Post)
                    .WithMany()
                    .HasForeignKey(n => n.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Comment)
                    .WithMany()
                    .HasForeignKey(n => n.CommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Group)
                    .WithMany()
                    .HasForeignKey(n => n.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== AUDIT LOG CONFIGURATION ==========
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(al => al.Admin)
                    .WithMany()
                    .HasForeignKey(al => al.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.AdminId, e.CreatedAt });
                entity.HasIndex(e => new { e.EntityType, e.EntityId });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== REPORTED CONTENT CONFIGURATION ==========
            modelBuilder.Entity<ReportedContent>(entity =>
            {
                entity.HasOne(rc => rc.Reporter)
                    .WithMany(u => u.Reports)
                    .HasForeignKey(rc => rc.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rc => rc.ResolvedBy)
                    .WithMany()
                    .HasForeignKey(rc => rc.ResolvedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.ContentType, e.ContentId });
                entity.HasIndex(e => new { e.ReporterId, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ========== SEED DATA ==========
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Group Roles
            modelBuilder.Entity<GroupRole>().HasData(
                new GroupRole
                {
                    Id = 1,
                    Name = "Owner",
                    Description = "Chủ nhóm - Có toàn quyền quản lý nhóm"
                },
                new GroupRole
                {
                    Id = 2,
                    Name = "Admin",
                    Description = "Quản trị viên - Quản lý thành viên và bài viết"
                },
                new GroupRole
                {
                    Id = 3,
                    Name = "Member",
                    Description = "Thành viên - Tham gia và đăng bài trong nhóm"
                }
            );
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
