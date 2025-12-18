using HealthCareBlog_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HealthCareBlog_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
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

        // System
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ReportedContent> ReportedContents => Set<ReportedContent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== USER CONFIGURATION ==========
            modelBuilder.Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

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
            });

            // ========== POST CONFIGURATION ==========
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.LikeCount).HasDefaultValue(0);
                entity.Property(e => e.CommentCount).HasDefaultValue(0);
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
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(l => l.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.PostId, e.CommentId }).IsUnique();
            });

            // ========== COMMENT CONFIGURATION ==========
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LikeCount).HasDefaultValue(0);
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

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => new { e.UserId, e.IsRead });
            });

            // ========== AUDIT LOG CONFIGURATION ==========
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(al => al.Admin)
                    .WithMany()
                    .HasForeignKey(al => al.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => new { e.EntityType, e.EntityId });
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
                entity.HasIndex(e => new { e.ContentType, e.ContentId });
            });
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultSQLConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultSQLConnection' not found. " +
                    "Please ensure it is properly configured in appsettings.json");
            }

            optionsBuilder.UseSqlServer(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
