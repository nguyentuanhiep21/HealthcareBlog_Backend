using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HealthCareBlog_Backend.Services;

/// <summary>
/// ReportService — business logic cho Report module.
/// ApplicationDbContext thay bằng IReportRepository.
/// UserManager giữ nguyên cho các Identity operations (lock, delete user).
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly UserManager<User> _userManager;

    public ReportService(IReportRepository reportRepository, UserManager<User> userManager)
    {
        _reportRepository = reportRepository;
        _userManager = userManager;
    }

    private async Task<(User? targetUser, string? contentPreview)> GetTargetInfoAsync(string contentType, string contentId)
    {
        return contentType switch
        {
            "User" => (await _reportRepository.GetUserByIdAsync(contentId), null),
            "Post" when int.TryParse(contentId, out var postId) => await _reportRepository.GetPostTargetInfoAsync(postId),
            "Comment" when int.TryParse(contentId, out var commentId) => await _reportRepository.GetCommentTargetInfoAsync(commentId),
            _ => (null, null)
        };
    }

    public async Task<(ViewReportDTO report, bool isExisting)> CreateReportAsync(string reporterId, CreateReportDTO createReportDTO)
    {
        if (createReportDTO == null)
            throw new BadRequestException("Dữ liệu không hợp lệ.");

        var validContentTypes = new[] { "Post", "Comment", "User" };
        if (!validContentTypes.Contains(createReportDTO.ContentType))
            throw new BadRequestException("Loại nội dung không hợp lệ. Phải là Post, Comment hoặc User.");

        // Kiểm tra content tồn tại
        var exists = createReportDTO.ContentType switch
        {
            "Post" => await _reportRepository.PostExistsAsync(createReportDTO.ContentId),
            "Comment" => await _reportRepository.CommentExistsAsync(createReportDTO.ContentId),
            "User" => await _reportRepository.UserExistsAsync(createReportDTO.ContentId),
            _ => false
        };

        if (!exists)
            throw new NotFoundException($"Không tìm thấy {createReportDTO.ContentType.ToLower()} được báo cáo.");

        var existingReport = await _reportRepository.GetExistingPendingAsync(
            reporterId, createReportDTO.ContentType, createReportDTO.ContentId);

        var reporter = await _reportRepository.GetUserByIdAsync(reporterId);
        var (targetUser, contentPreview) = await GetTargetInfoAsync(createReportDTO.ContentType, createReportDTO.ContentId);

        if (existingReport != null)
            return (existingReport.ToViewReportDTO(reporter, targetUser, contentPreview), true);

        var newReport = new ReportedContent
        {
            ContentType = createReportDTO.ContentType,
            ContentId = createReportDTO.ContentId,
            Reason = createReportDTO.Reason,
            Description = createReportDTO.Description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ReporterId = reporterId,
            ReporterFullName = reporter?.FullName,
            TargetUserId = targetUser?.Id,
            TargetUserFullName = targetUser?.FullName,
            TargetContentSnapshot = contentPreview
        };

        await _reportRepository.AddAsync(newReport);
        await _reportRepository.SaveChangesAsync();

        return (newReport.ToViewReportDTO(reporter, targetUser, contentPreview), false);
    }

    public async Task<List<ViewReportDTO>> GetAllReportsAsync(int page = 1, int pageSize = 20, string? status = null, string? contentType = null)
    {
        var reports = await _reportRepository.GetPagedAsync(page, pageSize, status, contentType);
        var result = new List<ViewReportDTO>();

        foreach (var report in reports)
        {
            User? reporter = string.IsNullOrEmpty(report.ReporterId)
                ? null
                : await _reportRepository.GetUserByIdAsync(report.ReporterId);
            var (targetUser, contentPreview) = await GetTargetInfoAsync(report.ContentType, report.ContentId);
            result.Add(report.ToViewReportDTO(reporter, targetUser, contentPreview));
        }

        return result;
    }

    public async Task<ViewReportDTO> ProcessUserReportAsync(string adminId, int reportId, ProcessUserReportDTO processReportDTO)
    {
        var report = await _reportRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException("Không tìm thấy báo cáo.");

        if (report.Status != "Pending") throw new BadRequestException("Báo cáo này đã được xử lý.");
        if (report.ContentType != "User") throw new BadRequestException("Endpoint này chỉ xử lý báo cáo người dùng.");

        var validActions = new[] { "Lock", "Delete", "Ignore" };
        if (!validActions.Contains(processReportDTO.Action))
            throw new BadRequestException("Hành động không hợp lệ. Phải là Lock, Delete hoặc Ignore.");

        var targetUser = await _reportRepository.GetUserByIdAsync(report.ContentId)
            ?? throw new NotFoundException("Không tìm thấy người dùng bị báo cáo.");

        var targetUserRoles = await _userManager.GetRolesAsync(targetUser);
        if (targetUserRoles.Contains("Admin"))
            throw new BadRequestException("Không thể xử lý báo cáo đối với quản trị viên.");

        var reporter = await _reportRepository.GetUserByIdAsync(report.ReporterId!);
        var targetUserName = targetUser.FullName ?? targetUser.UserName ?? "Unknown";

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
                var userPosts = await _reportRepository.GetPostsByUserIdAsync(targetUser.Id);
                var postIds = userPosts.Select(p => p.Id).ToList();
                var commentIdsOnPosts = postIds.Any() ? await _reportRepository.GetCommentIdsByPostIdsAsync(postIds) : new List<int>();
                var userCommentIds = await _reportRepository.GetCommentIdsByUserIdAsync(targetUser.Id);

                if (commentIdsOnPosts.Any())
                {
                    await _reportRepository.RemoveCommentLikesByCommentIdsAsync(commentIdsOnPosts);
                    await _reportRepository.RemoveCommentsByPostIdsAsync(postIds);
                }
                if (postIds.Any())
                {
                    await _reportRepository.RemovePostLikesByPostIdsAsync(postIds);
                    await _reportRepository.RemoveSavedPostsByPostIdsAsync(postIds);
                    await _reportRepository.RemoveNotificationsByPostIdsAsync(postIds);
                    await _reportRepository.RemovePostsByUserIdAsync(targetUser.Id);
                }
                if (userCommentIds.Any())
                {
                    await _reportRepository.RemoveCommentLikesByCommentIdsAsync(userCommentIds);
                    await _reportRepository.RemoveCommentsByUserIdAsync(userCommentIds);
                }

                await _reportRepository.RemovePostLikesByUserIdAsync(targetUser.Id);
                await _reportRepository.RemoveCommentLikesByUserIdAsync(targetUser.Id);
                await _reportRepository.RemoveFollowsByUserIdAsync(targetUser.Id);
                await _reportRepository.RemoveSavedPostsByUserIdAsync(targetUser.Id);
                await _reportRepository.RemoveNotificationsByUserIdAsync(targetUser.Id, postIds, commentIdsOnPosts.Concat(userCommentIds));
                await _reportRepository.RemoveReportsByUserIdExceptCurrentAsync(targetUser.Id, reportId);

                await _reportRepository.SaveChangesAsync();
                await _userManager.DeleteAsync(targetUser);
                report.Status = "Resolved";
                break;

            case "Ignore":
                report.Status = "Rejected";
                break;
        }

        report.ResolvedAt = DateTime.UtcNow;
        report.ResolvedById = adminId;
        await _reportRepository.SaveChangesAsync();

        return report.ToViewReportDTO(
            reporter,
            processReportDTO.Action == "Delete" ? null : targetUser,
            processReportDTO.Action == "Delete" ? $"User: {targetUserName}" : targetUser.Email
        );
    }

    public async Task<ViewReportDTO> ProcessPostReportAsync(string adminId, int reportId, ProcessPostReportDTO processReportDTO)
    {
        var report = await _reportRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException("Không tìm thấy báo cáo.");

        if (report.Status != "Pending") throw new BadRequestException("Báo cáo này đã được xử lý.");
        if (report.ContentType != "Post") throw new BadRequestException("Endpoint này chỉ xử lý báo cáo bài viết.");

        var validActions = new[] { "Delete", "Ignore" };
        if (!validActions.Contains(processReportDTO.Action))
            throw new BadRequestException("Hành động không hợp lệ. Phải là Delete hoặc Ignore.");

        var reporter = await _reportRepository.GetUserByIdAsync(report.ReporterId!);
        var (postAuthor, postContent) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

        if (processReportDTO.Action == "Delete" && int.TryParse(report.ContentId, out var postId))
        {
            var post = await _reportRepository.GetPostByIdAsync(postId);
            if (post != null)
            {
                var commentIds = await _reportRepository.GetCommentIdsByPostIdsAsync(new[] { postId });
                if (commentIds.Any())
                {
                    await _reportRepository.RemoveCommentLikesByCommentIdsAsync(commentIds);
                    await _reportRepository.RemoveNotificationsByCommentIdsAsync(commentIds);
                    await _reportRepository.RemoveCommentsByPostIdsAsync(new[] { postId });
                }
                await _reportRepository.RemovePostLikesByPostIdsAsync(new[] { postId });
                await _reportRepository.RemoveSavedPostsByPostIdsAsync(new[] { postId });
                await _reportRepository.RemoveNotificationsByPostIdsAsync(new[] { postId });
                await _reportRepository.RemovePostAsync(post);
            }
            report.Status = "Resolved";
        }
        else
        {
            report.Status = "Rejected";
        }

        report.ResolvedAt = DateTime.UtcNow;
        report.ResolvedById = adminId;
        await _reportRepository.SaveChangesAsync();

        return report.ToViewReportDTO(
            reporter,
            processReportDTO.Action == "Delete" ? null : postAuthor,
            processReportDTO.Action == "Delete" ? $"Post: {postContent}" : postContent
        );
    }

    public async Task<ViewReportDTO> ProcessCommentReportAsync(string adminId, int reportId, ProcessCommentReportDTO processReportDTO)
    {
        var report = await _reportRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException("Không tìm thấy báo cáo.");

        if (report.Status != "Pending") throw new BadRequestException("Báo cáo này đã được xử lý.");
        if (report.ContentType != "Comment") throw new BadRequestException("Endpoint này chỉ xử lý báo cáo bình luận.");

        var validActions = new[] { "Delete", "Ignore" };
        if (!validActions.Contains(processReportDTO.Action))
            throw new BadRequestException("Hành động không hợp lệ. Phải là Delete hoặc Ignore.");

        var reporter = await _reportRepository.GetUserByIdAsync(report.ReporterId!);
        var (commentAuthor, commentContent) = await GetTargetInfoAsync(report.ContentType, report.ContentId);

        if (processReportDTO.Action == "Delete" && int.TryParse(report.ContentId, out var commentId))
        {
            var comment = await _reportRepository.GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                await _reportRepository.RemoveCommentLikesByCommentIdsAsync(new[] { commentId });
                await _reportRepository.RemoveNotificationsByCommentIdsAsync(new[] { commentId });
                await _reportRepository.RemoveCommentAsync(comment);

                var post = await _reportRepository.GetPostForCommentAsync(comment.PostId);
                if (post != null)
                    post.CommentCount = Math.Max(0, post.CommentCount - 1);
            }
            report.Status = "Resolved";
        }
        else
        {
            report.Status = "Rejected";
        }

        report.ResolvedAt = DateTime.UtcNow;
        report.ResolvedById = adminId;
        await _reportRepository.SaveChangesAsync();

        return report.ToViewReportDTO(
            reporter,
            processReportDTO.Action == "Delete" ? null : commentAuthor,
            processReportDTO.Action == "Delete" ? $"Comment: {commentContent}" : commentContent
        );
    }
}
