using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs.Reports;
using HealthCareBlog_Backend.Services.Interfaces;
using HealthCareBlog_Backend.Exceptions;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Models.Mapper;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
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

            if (existingReport != null)
            {
                // Return existing report with flag indicating it already exists
                return (ReportMapper.ToViewReportDTO(existingReport), true);
            }

            var newReport = new ReportedContent
            {
                ContentType = createReportDTO.ContentType,
                ContentId = createReportDTO.ContentId,
                Reason = createReportDTO.Reason,
                Description = createReportDTO.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                ReporterId = reporterId
            };

            _context.ReportedContents.Add(newReport);
            await _context.SaveChangesAsync();

            return (newReport.ToViewReportDTO(), false);
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

            return reports.Select(r => r.ToViewReportDTO()).ToList();
        }

        public async Task<ViewReportDTO> GetReportByIdAsync(int reportId)
        {
            var report = await _context.ReportedContents.FindAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException("Report not found.");
            }

            return report.ToViewReportDTO();
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

            return reports.Select(r => r.ToViewReportDTO()).ToList();
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

            return report.ToViewReportDTO();
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
    }
}
