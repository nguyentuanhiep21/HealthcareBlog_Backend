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
        public DbSet<SavedPost> SavedPosts => Set<SavedPost>();

        // Post & Content
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<LikePost> LikePosts => Set<LikePost>();
        public DbSet<LikeComment> LikeComments => Set<LikeComment>();
        public DbSet<Comment> Comments => Set<Comment>();

        // System
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<ReportedContent> ReportedContents => Set<ReportedContent>();
        public DbSet<ReportedContent> Reports => Set<ReportedContent>(); // Alias for ReportedContents

        // Nutrition
        public DbSet<NutritionProfile> NutritionProfiles => Set<NutritionProfile>();
        public DbSet<NutritionChatSession> NutritionChatSessions => Set<NutritionChatSession>();
        public DbSet<NutritionChatMessage> NutritionChatMessages => Set<NutritionChatMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default schema for EF migrations
            modelBuilder.HasDefaultSchema("public");

            // ========== USER CONFIGURATION ==========
            modelBuilder.Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FollowerCount).HasDefaultValue(0);
                entity.Property(e => e.FollowingCount).HasDefaultValue(0);
            });

            // ========== FOLLOW CONFIGURATION ==========
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(f => f.FollowingUser)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            });

            // ========== SAVED POST CONFIGURATION ==========
            modelBuilder.Entity<SavedPost>(entity =>
            {
                entity.HasOne(sp => sp.User)
                    .WithMany(u => u.SavedPosts)
                    .HasForeignKey(sp => sp.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(sp => sp.Post)
                    .WithMany(p => p.SavedByUsers)
                    .HasForeignKey(sp => sp.PostId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // ========== POST CONFIGURATION ==========
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.LikeCount).HasDefaultValue(0);
                entity.Property(e => e.CommentCount).HasDefaultValue(0);
            });

            // ========== COMMENT CONFIGURATION ==========
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LikeCount).HasDefaultValue(0);
            });

            // ========== LIKE POST CONFIGURATION ==========
            modelBuilder.Entity<LikePost>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.LikePosts)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // ========== LIKE COMMENT CONFIGURATION ==========
            modelBuilder.Entity<LikeComment>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.LikeComments)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(l => l.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(l => l.CommentId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => new { e.UserId, e.CommentId }).IsUnique();
                entity.HasIndex(e => e.CommentId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // ========== NOTIFICATION CONFIGURATION ==========
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.ReceivedNotifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(n => n.Actor)
                    .WithMany()
                    .HasForeignKey(n => n.ActorId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(n => n.Post)
                    .WithMany()
                    .HasForeignKey(n => n.PostId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasOne(n => n.Comment)
                    .WithMany()
                    .HasForeignKey(n => n.CommentId)
                    .OnDelete(DeleteBehavior.Restrict); // Xử lý trong code

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => new { e.UserId, e.IsRead });
            });

            // ========== REPORTED CONTENT CONFIGURATION ==========
            modelBuilder.Entity<ReportedContent>(entity =>
            {
                entity.HasOne(rc => rc.Reporter)
                    .WithMany(u => u.Reports)
                    .HasForeignKey(rc => rc.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict); // Giữ lại reports

                entity.HasOne(rc => rc.ResolvedBy)
                    .WithMany()
                    .HasForeignKey(rc => rc.ResolvedById)
                    .OnDelete(DeleteBehavior.Restrict); // Giữ lại reports

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.ContentType, e.ContentId });
            });

            // ========== NUTRITION PROFILE CONFIGURATION ==========
            modelBuilder.Entity<NutritionProfile>(entity =>
            {
                entity.HasOne(np => np.User)
                    .WithMany()
                    .HasForeignKey(np => np.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(np => np.ActiveChatSession)
                    .WithOne(ncs => ncs.NutritionProfile)
                    .HasForeignKey<NutritionChatSession>(ncs => ncs.NutritionProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId).IsUnique(); // One profile per user
            });

            // ========== NUTRITION CHAT SESSION CONFIGURATION ==========
            modelBuilder.Entity<NutritionChatSession>(entity =>
            {
                entity.HasIndex(e => e.NutritionProfileId).IsUnique(); // One session per profile
                entity.HasIndex(e => e.CreatedAt);
            });

            // ========== NUTRITION CHAT MESSAGE CONFIGURATION ==========
            modelBuilder.Entity<NutritionChatMessage>(entity =>
            {
                entity.HasOne(ncm => ncm.Session)
                    .WithMany(ncs => ncs.Messages)
                    .HasForeignKey(ncm => ncm.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SessionId);
                entity.HasIndex(e => e.CreatedAt);
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
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnectionString' not found. " +
                    "Please ensure it is properly configured in appsettings.json");
            }

            optionsBuilder.UseNpgsql(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
