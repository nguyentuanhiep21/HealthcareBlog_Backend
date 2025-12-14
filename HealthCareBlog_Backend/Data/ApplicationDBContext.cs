using HealthCareBlog_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                entity.HasOne(p => p.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.UserId, e.CreatedAt });

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Counters default
                entity.Property(e => e.LikeCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.CommentCount)
                    .HasDefaultValue(0);
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

                // Counter default
                entity.Property(e => e.LikeCount)
                    .HasDefaultValue(0);
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
            // No seed data required for current schema
        }

        // ========== SAVE CHANGES HOOKS TO MAINTAIN COUNTERS ==========
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            // Process counter updates synchronously
            ProcessCountersAsync(false).GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await ProcessCountersAsync(true);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private async Task ProcessCountersAsync(bool isAsync)
        {
            // Likes
            var addedLikes = ChangeTracker.Entries<Like>().Where(e => e.State == EntityState.Added).ToList();
            var deletedLikes = ChangeTracker.Entries<Like>().Where(e => e.State == EntityState.Deleted).ToList();

            // Comments
            var addedComments = ChangeTracker.Entries<Comment>().Where(e => e.State == EntityState.Added).ToList();
            var deletedComments = ChangeTracker.Entries<Comment>().Where(e => e.State == EntityState.Deleted).ToList();
            var modifiedComments = ChangeTracker.Entries<Comment>().Where(e => e.State == EntityState.Modified).ToList();

            // Process added likes
            foreach (var entry in addedLikes)
            {
                var postId = entry.Entity.PostId;
                var commentId = entry.Entity.CommentId;

                if (postId.HasValue && postId.Value != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET like_count = ISNULL(like_count,0) + 1 WHERE id = {0}", postId.Value);
                    else Database.ExecuteSqlRaw("UPDATE posts SET like_count = ISNULL(like_count,0) + 1 WHERE id = {0}", postId.Value);

                    var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == postId.Value);
                    if (postEntry != null) postEntry.Entity.LikeCount++;
                }

                if (commentId.HasValue && commentId.Value != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE comments SET like_count = ISNULL(like_count,0) + 1 WHERE id = {0}", commentId.Value);
                    else Database.ExecuteSqlRaw("UPDATE comments SET like_count = ISNULL(like_count,0) + 1 WHERE id = {0}", commentId.Value);

                    var commentEntry = ChangeTracker.Entries<Comment>().FirstOrDefault(ce => ce.Entity.Id == commentId.Value);
                    if (commentEntry != null) commentEntry.Entity.LikeCount++;
                }
            }

            // Process deleted likes
            foreach (var entry in deletedLikes)
            {
                var origPostId = entry.OriginalValues.GetValue<int?>(nameof(Like.PostId));
                var origCommentId = entry.OriginalValues.GetValue<int?>(nameof(Like.CommentId));

                if (origPostId.HasValue && origPostId.Value != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET like_count = CASE WHEN like_count > 0 THEN like_count - 1 ELSE 0 END WHERE id = {0}", origPostId.Value);
                    else Database.ExecuteSqlRaw("UPDATE posts SET like_count = CASE WHEN like_count > 0 THEN like_count - 1 ELSE 0 END WHERE id = {0}", origPostId.Value);

                    var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == origPostId.Value);
                    if (postEntry != null && postEntry.Entity.LikeCount > 0) postEntry.Entity.LikeCount--;
                }

                if (origCommentId.HasValue && origCommentId.Value != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE comments SET like_count = CASE WHEN like_count > 0 THEN like_count - 1 ELSE 0 END WHERE id = {0}", origCommentId.Value);
                    else Database.ExecuteSqlRaw("UPDATE comments SET like_count = CASE WHEN like_count > 0 THEN like_count - 1 ELSE 0 END WHERE id = {0}", origCommentId.Value);

                    var commentEntry = ChangeTracker.Entries<Comment>().FirstOrDefault(ce => ce.Entity.Id == origCommentId.Value);
                    if (commentEntry != null && commentEntry.Entity.LikeCount > 0) commentEntry.Entity.LikeCount--;
                }
            }

            // Process added comments
            foreach (var entry in addedComments)
            {
                var postId = entry.Entity.PostId;
                if (postId != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET comment_count = ISNULL(comment_count,0) + 1 WHERE id = {0}", postId);
                    else Database.ExecuteSqlRaw("UPDATE posts SET comment_count = ISNULL(comment_count,0) + 1 WHERE id = {0}", postId);

                    var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == postId);
                    if (postEntry != null) postEntry.Entity.CommentCount++;
                }
            }

            // Process deleted comments
            foreach (var entry in deletedComments)
            {
                var origPostId = entry.OriginalValues.GetValue<int>(nameof(Comment.PostId));
                if (origPostId != 0)
                {
                    if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET comment_count = CASE WHEN comment_count > 0 THEN comment_count - 1 ELSE 0 END WHERE id = {0}", origPostId);
                    else Database.ExecuteSqlRaw("UPDATE posts SET comment_count = CASE WHEN comment_count > 0 THEN comment_count - 1 ELSE 0 END WHERE id = {0}", origPostId);

                    var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == origPostId);
                    if (postEntry != null && postEntry.Entity.CommentCount > 0) postEntry.Entity.CommentCount--;
                }
            }

            // Process modified comments for soft-delete changes
            foreach (var entry in modifiedComments)
            {
                var origIsDeleted = entry.OriginalValues.GetValue<bool>(nameof(Comment.IsDeleted));
                var curIsDeleted = entry.Entity.IsDeleted;

                if (!origIsDeleted && curIsDeleted)
                {
                    var postId = entry.Entity.PostId;
                    if (postId != 0)
                    {
                        if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET comment_count = CASE WHEN comment_count > 0 THEN comment_count - 1 ELSE 0 END WHERE id = {0}", postId);
                        else Database.ExecuteSqlRaw("UPDATE posts SET comment_count = CASE WHEN comment_count > 0 THEN comment_count - 1 ELSE 0 END WHERE id = {0}", postId);

                        var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == postId);
                        if (postEntry != null && postEntry.Entity.CommentCount > 0) postEntry.Entity.CommentCount--;
                    }
                }
                else if (origIsDeleted && !curIsDeleted)
                {
                    var postId = entry.Entity.PostId;
                    if (postId != 0)
                    {
                        if (isAsync) await Database.ExecuteSqlRawAsync("UPDATE posts SET comment_count = ISNULL(comment_count,0) + 1 WHERE id = {0}", postId);
                        else Database.ExecuteSqlRaw("UPDATE posts SET comment_count = ISNULL(comment_count,0) + 1 WHERE id = {0}", postId);

                        var postEntry = ChangeTracker.Entries<Post>().FirstOrDefault(pe => pe.Entity.Id == postId);
                        if (postEntry != null) postEntry.Entity.CommentCount++;
                    }
                }
            }
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
