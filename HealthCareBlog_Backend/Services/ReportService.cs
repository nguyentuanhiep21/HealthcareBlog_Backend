using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HealthCareBlog_Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReportService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Helper method to get target user and content preview based on ContentType
        /// If original content is deleted, returns null so snapshot from ReportedContent will be used
        /// </summary>
        private async Task<(User? targetUser, string? contentPreview)> GetTargetInfoAsync(string contentType, string contentId)
        {
            User? targetUser = null;
            string? contentPreview = null;

            switch (contentType)
            {
                case "User":
                    targetUser = await _context.Users.FindAsync(contentId);
                    contentPreview = targetUser?.Email;
                    break;

                case "Post":
                    if (int.TryParse(contentId, out int postId))
                    {
                        var post = await _context.Posts
                            .Include(p => p.User)
                            .FirstOrDefaultAsync(p => p.Id == postId);
                        if (post != null)
                        {
                            targetUser = post.User;
                            contentPreview = post.Content.Length > 100 
                                ? post.Content.Substring(0, 100) + "..." 
                                : post.Content;
                        }
                    }
                    break;

                case "Comment":
                    if (int.TryParse(contentId, out int commentId))
                    {
                        var comment = await _context.Comments
                            .Include(c => c.User)
                            .FirstOrDefaultAsync(c => c.Id == commentId);
                        if (comment != null)
                        {
                            targetUser = comment.User;
                            contentPreview = comment.Content.Length > 100 
                                ? comment.Content.Substring(0, 100) + "..." 
                                : comment.Content;
                        }
                    }
                    break;
            }

            return (targetUser, contentPreview);
        }

        public async Task<(ViewReportDTO report, bool isExisting)> CreateReportAsync(string reporterId, CreateReportDTO createReportDTO)
        {
            if (createReportDTO == null)
            {
                throw new BadRequestException("Dữ liệu không hợp lệ.");
            }

            var validContentTypes = new[] { "Post", "Comment", "User" };
            if (!validContentTypes.Contains(createReportDTO.ContentType))
            {
                throw new BadRequestException("Loại nội dung không hợp lệ. Phải là Post, Comment hoặc User.");
            }

            if (createReportDTO.ContentType == "Post")
            {
                var postExists = await _context.Posts.AnyAsync(p => p.Id.ToString() == createReportDTO.ContentId);
                if (!postExists)
                {
                    throw new NotFoundException("Không tìm thấy bài viết.");
                }
            }
            else if (createReportDTO.ContentType == "Comment")
            {
                var commentExists = await _context.Comments.AnyAsync(c => c.Id.ToString() == createReportDTO.ContentId);
                if (!commentExists)
                {
                    throw new NotFoundException("Không tìm thấy bình luận.");
                }
            }
            else if (createReportDTO.ContentType == "User")
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == createReportDTO.ContentId);
                if (!userExists)
                {
                    throw new NotFoundException("Không tìm thấy người dùng.");
                }
            }

            var existingReport = await _context.ReportedContents
                .FirstOrDefaultAsync(r => r.ReporterId == reporterId 
                    && r.ContentType == createReportDTO.ContentType 
                    && r.ContentId == createReportDTO.ContentId 
                    && r.Status == "Pending");

            // Get reporter and target info
            var reporter = await _context.Users.FindAsync(reporterId);
            var (targetUser, contentPreview) = await GetTargetInfoAsync(createReportDTO.ContentType, createReportDTO.ContentId);

            if (existingReport != null)
            {
                return (existingReport.ToViewReportDTO(reporter, targetUser, contentPreview), true);
            }

            var newReport = new ReportedContent
            {
                ContentType = createReportDTO.ContentType,
                ContentId = createReportDTO.ContentId,
                Reason = createReportDTO.Reason,
                Description = createReportDTO.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                ReporterId = reporterId,
                // Save snapshot
                ReporterFullName = reporter?.FullName,
                TargetUserId = targetUser?.Id,
                TargetUserFullName = targetUser?.FullName,
                TargetContentSnapshot = contentPreview
            };

            _context.ReportedContents.Add(newReport);
            await _context.SaveChangesAsync();

            return (newReport.ToViewReportDTO(reporter, targetUser, contentPreview), false);
        }

        public async Task<List<ViewReportDTO>> GetAllReportsAsync(int page = 1, int pageSize = 20, string? status = null, string? contentType = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.ReportedContents.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                query = query.Where(r => r.ContentType == contentType);
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ViewReportDTO>();
            foreach (var report in reports)
            {
                User? reporter = null;
                if (!string.IsNullOrEmpty(report.ReporterId))
                {
                    reporter = await _context.Users.FindAsync(report.ReporterId);
                }
                var (targetUser, contentPreview) = await GetTargetInfoAsync(report.ContentType, report.ContentId);
                    
                result.Add(report.ToViewReportDTO(reporter, targetUser, contentPreview));
            }

            return result;
        }

        public async Task<ViewReportDTO> GetReportByIdAsync(int reportId)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Report not found.");
            }

            User? reporter = null;
            if (!string.IsNullOrEmpty(report.ReporterId))
            {
                reporter = await _context.Users.FindAsync(report.ReporterId);
            }
            var (targetUser, contentPreview) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

            return report.ToViewReportDTO(reporter, targetUser, contentPreview);
        }

        public async Task<List<ViewReportDTO>> GetUserReportsAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var reports = await _context.ReportedContents
                .Where(r => r.ReporterId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ViewReportDTO>();
            foreach (var report in reports)
            {
                User? reporter = null;
                if (!string.IsNullOrEmpty(report.ReporterId))
                {
                    reporter = await _context.Users.FindAsync(report.ReporterId);
                }
                var (targetUser, contentPreview) = await GetTargetInfoAsync(report.ContentType, report.ContentId);
                    
                result.Add(report.ToViewReportDTO(reporter, targetUser, contentPreview));
            }

            return result;
        }

        public async Task<ViewReportDTO> ResolveReportAsync(string adminId, int reportId, ResolveReportDTO resolveReportDTO)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Report not found.");
            }

            if (report.Status != "Pending")
            {
                throw new BadRequestException("Report has already been resolved.");
            }

            var validStatuses = new[] { "Resolved", "Rejected" };
            if (!validStatuses.Contains(resolveReportDTO.Status))
            {
                throw new BadRequestException("Invalid status. Must be Resolved or Rejected.");
            }

            report.Status = resolveReportDTO.Status;
            report.AdminNote = resolveReportDTO.AdminNote;
            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedById = adminId;

            await _context.SaveChangesAsync();

            var reporter = await _context.Users.FindAsync(report.ReporterId);
            var (targetUser, contentPreview) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

            return report.ToViewReportDTO(reporter, targetUser, contentPreview);
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Report not found.");
            }

            _context.ReportedContents.Remove(report);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ViewReportDTO> ProcessUserReportAsync(string adminId, int reportId, ProcessUserReportDTO processReportDTO)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Không tìm thấy báo cáo.");
            }

            if (report.Status != "Pending")
            {
                throw new BadRequestException("Báo cáo này đã được xử lý.");
            }

            if (report.ContentType != "User")
            {
                throw new BadRequestException("Endpoint này chỉ xử lý báo cáo người dùng.");
            }

            var validActions = new[] { "Lock", "Delete", "Ignore" };
            if (!validActions.Contains(processReportDTO.Action))
            {
                throw new BadRequestException("Hành động không hợp lệ. Phải là Lock, Delete hoặc Ignore.");
            }

            var targetUser = await _context.Users.FindAsync(report.ContentId);
            if (targetUser == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng bị báo cáo.");
            }

            // Check if target user is an Admin
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);
            if (targetUserRoles.Contains("Admin"))
            {
                throw new BadRequestException("Không thể xử lý báo cáo đối với quản trị viên.");
            }

            // Get reporter info and target info BEFORE any deletion
            var reporter = await _context.Users.FindAsync(report.ReporterId);
            var targetUserName = targetUser.FullName ?? targetUser.UserName ?? "Unknown";
            var targetUserAvatar = targetUser.AvatarUrl;
            var targetUserId = targetUser.Id;

            // Process action
            switch (processReportDTO.Action)
            {
                case "Lock":
                    targetUser.IsLocked = true;
                    targetUser.LockedAt = DateTime.UtcNow;
                    targetUser.LockReason = $"Vi phạm: {report.Reason}";
                    await _userManager.UpdateAsync(targetUser);
                    report.Status = "Resolved";
                    break;

                case "Delete":
                    // Get all posts and comments IDs before deletion
                    var userPosts = await _context.Posts.Where(p => p.UserId == targetUser.Id).ToListAsync();
                    var userPostIds = userPosts.Select(p => p.Id).ToList();
                    
                    var userComments = await _context.Comments.Where(c => c.UserId == targetUser.Id).ToListAsync();
                    var userCommentIds = userComments.Select(c => c.Id).ToList();

                    // Delete comments on user's posts (including their likes and notifications)
                    if (userPostIds.Any())
                    {
                        var commentsOnUserPosts = await _context.Comments.Where(c => userPostIds.Contains(c.PostId)).ToListAsync();
                        var commentIdsOnUserPosts = commentsOnUserPosts.Select(c => c.Id).ToList();
                        
                        // Delete likes on these comments
                        if (commentIdsOnUserPosts.Any())
                        {
                            var likesOnComments = await _context.LikeComments.Where(l => commentIdsOnUserPosts.Contains(l.CommentId)).ToListAsync();
                            _context.LikeComments.RemoveRange(likesOnComments);
                        }
                        
                        _context.Comments.RemoveRange(commentsOnUserPosts);
                    }

                    // Delete likes on user's posts
                    if (userPostIds.Any())
                    {
                        var postLikes = await _context.LikePosts.Where(l => userPostIds.Contains(l.PostId)).ToListAsync();
                        _context.LikePosts.RemoveRange(postLikes);
                    }

                    // Delete saved posts of user's posts
                    if (userPostIds.Any())
                    {
                        var savedPosts = await _context.SavedPosts.Where(sp => userPostIds.Contains(sp.PostId)).ToListAsync();
                        _context.SavedPosts.RemoveRange(savedPosts);
                    }

                    // Delete user's posts
                    _context.Posts.RemoveRange(userPosts);

                    // Delete likes on user's comments
                    if (userCommentIds.Any())
                    {
                        var commentLikes = await _context.LikeComments.Where(l => userCommentIds.Contains(l.CommentId)).ToListAsync();
                        _context.LikeComments.RemoveRange(commentLikes);
                    }

                    // Decrease comment count for posts that have user's comments
                    if (userComments.Any())
                    {
                        var postsWithUserComments = userComments.GroupBy(c => c.PostId).ToList();
                        foreach (var group in postsWithUserComments)
                        {
                            var post = await _context.Posts.FindAsync(group.Key);
                            if (post != null && !userPostIds.Contains(post.Id)) // Don't update if post is also being deleted
                            {
                                post.CommentCount = Math.Max(0, post.CommentCount - group.Count());
                            }
                        }
                    }

                    // Delete user's comments
                    _context.Comments.RemoveRange(userComments);

                    // Delete likes made by user
                    var userLikePosts = await _context.LikePosts.Where(l => l.UserId == targetUser.Id).ToListAsync();
                    _context.LikePosts.RemoveRange(userLikePosts);
                    
                    var userLikeComments = await _context.LikeComments.Where(l => l.UserId == targetUser.Id).ToListAsync();
                    _context.LikeComments.RemoveRange(userLikeComments);

                    // Delete user's follows
                    var userFollows = await _context.Follows
                        .Where(f => f.FollowerId == targetUser.Id || f.FollowingId == targetUser.Id)
                        .ToListAsync();
                    _context.Follows.RemoveRange(userFollows);

                    // Delete user's saved posts
                    var userSavedPosts = await _context.SavedPosts.Where(sp => sp.UserId == targetUser.Id).ToListAsync();
                    _context.SavedPosts.RemoveRange(userSavedPosts);

                    // Delete all notifications related to user, their posts, and their comments
                    var userNotifications = await _context.Notifications
                        .Where(n => n.UserId == targetUser.Id 
                                 || n.ActorId == targetUser.Id
                                 || (n.PostId.HasValue && userPostIds.Contains(n.PostId.Value))
                                 || (n.CommentId.HasValue && userCommentIds.Contains(n.CommentId.Value)))
                        .ToListAsync();
                    _context.Notifications.RemoveRange(userNotifications);

                    // Delete reports created by this user (except the current one being processed)
                    var userReports = await _context.ReportedContents
                        .Where(r => r.ReporterId == targetUser.Id && r.Id != reportId)
                        .ToListAsync();
                    _context.ReportedContents.RemoveRange(userReports);

                    // Delete user (keep reports with snapshot for history)
                    await _userManager.DeleteAsync(targetUser);
                    report.Status = "Resolved";
                    break;

                case "Ignore":
                    report.Status = "Rejected";
                    break;
            }

            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedById = adminId;

            await _context.SaveChangesAsync();

            // Return with saved target info (user might be deleted)
            return report.ToViewReportDTO(
                reporter, 
                processReportDTO.Action == "Delete" ? null : targetUser,
                processReportDTO.Action == "Delete" ? $"User: {targetUserName}" : targetUser.Email
            );
        }

        public async Task<ViewReportDTO> ProcessPostReportAsync(string adminId, int reportId, ProcessPostReportDTO processReportDTO)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Không tìm thấy báo cáo.");
            }

            if (report.Status != "Pending")
            {
                throw new BadRequestException("Báo cáo này đã được xử lý.");
            }

            if (report.ContentType != "Post")
            {
                throw new BadRequestException("Endpoint này chỉ xử lý báo cáo bài viết.");
            }

            var validActions = new[] { "Delete", "Ignore" };
            if (!validActions.Contains(processReportDTO.Action))
            {
                throw new BadRequestException("Hành động không hợp lệ. Phải là Delete hoặc Ignore.");
            }

            // Get post and reporter info BEFORE deletion
            var reporter = await _context.Users.FindAsync(report.ReporterId);
            var (postAuthor, postContent) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

            // Process action and update report status
            if (processReportDTO.Action == "Delete")
            {
                if (int.TryParse(report.ContentId, out int postId))
                {
                    var post = await _context.Posts.FindAsync(postId);
                    
                    if (post != null)
                    {
                        // Delete post's comments
                        var postComments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
                        var commentIds = postComments.Select(c => c.Id).ToList();
                        _context.Comments.RemoveRange(postComments);

                        // Delete post's likes
                        var postLikes = await _context.LikePosts.Where(l => l.PostId == postId).ToListAsync();
                        _context.LikePosts.RemoveRange(postLikes);

                        // Delete comment likes for all comments of this post
                        if (commentIds.Any())
                        {
                            var commentLikes = await _context.LikeComments.Where(l => commentIds.Contains(l.CommentId)).ToListAsync();
                            _context.LikeComments.RemoveRange(commentLikes);
                        }

                        // Delete post's saved
                        var savedPosts = await _context.SavedPosts.Where(sp => sp.PostId == postId).ToListAsync();
                        _context.SavedPosts.RemoveRange(savedPosts);

                        // Delete notifications related to post
                        var postNotifications = await _context.Notifications.Where(n => n.PostId == postId).ToListAsync();
                        _context.Notifications.RemoveRange(postNotifications);

                        // Delete notifications related to comments of this post
                        if (commentIds.Any())
                        {
                            var commentNotifications = await _context.Notifications.Where(n => n.CommentId.HasValue && commentIds.Contains(n.CommentId.Value)).ToListAsync();
                            _context.Notifications.RemoveRange(commentNotifications);
                        }

                        // Delete post (keep reports with snapshot for history)
                        _context.Posts.Remove(post);
                    }
                }
                report.Status = "Resolved";
            }
            else // Ignore
            {
                report.Status = "Rejected";
            }

            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedById = adminId;

            await _context.SaveChangesAsync();

            return report.ToViewReportDTO(
                reporter,
                processReportDTO.Action == "Delete" ? null : postAuthor,
                processReportDTO.Action == "Delete" ? $"Post: {postContent}" : postContent
            );
        }

        public async Task<ViewReportDTO> ProcessCommentReportAsync(string adminId, int reportId, ProcessCommentReportDTO processReportDTO)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Không tìm thấy báo cáo.");
            }

            if (report.Status != "Pending")
            {
                throw new BadRequestException("Báo cáo này đã được xử lý.");
            }

            if (report.ContentType != "Comment")
            {
                throw new BadRequestException("Endpoint này chỉ xử lý báo cáo bình luận.");
            }

            var validActions = new[] { "Delete", "Ignore" };
            if (!validActions.Contains(processReportDTO.Action))
            {
                throw new BadRequestException("Hành động không hợp lệ. Phải là Delete hoặc Ignore.");
            }

            // Get comment and reporter info BEFORE deletion
            var reporter = await _context.Users.FindAsync(report.ReporterId);
            var (commentAuthor, commentContent) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

            // Process action and update report status
            if (processReportDTO.Action == "Delete")
            {
                if (int.TryParse(report.ContentId, out int commentId))
                {
                    var comment = await _context.Comments.FindAsync(commentId);
                    
                    if (comment != null)
                    {
                        var postId = comment.PostId;

                        // Delete comment's likes
                        var commentLikes = await _context.LikeComments.Where(l => l.CommentId == commentId).ToListAsync();
                        _context.LikeComments.RemoveRange(commentLikes);

                        // Delete notifications related to comment
                        var commentNotifications = await _context.Notifications.Where(n => n.CommentId == commentId).ToListAsync();
                        _context.Notifications.RemoveRange(commentNotifications);

                        // Delete comment (keep reports with snapshot for history)
                        _context.Comments.Remove(comment);

                        // Decrease comment count of the post
                        var post = await _context.Posts.FindAsync(postId);
                        if (post != null)
                        {
                            post.CommentCount = Math.Max(0, post.CommentCount - 1);
                        }
                    }
                }
                report.Status = "Resolved";
            }
            else // Ignore
            {
                report.Status = "Rejected";
            }

            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedById = adminId;

            await _context.SaveChangesAsync();

            return report.ToViewReportDTO(
                reporter,
                processReportDTO.Action == "Delete" ? null : commentAuthor,
                processReportDTO.Action == "Delete" ? $"Comment: {commentContent}" : commentContent
            );
        }
    }
}
