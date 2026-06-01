using HealthCareBlog_Backend.Models.DTOs;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface IHealthAssessmentService
    {
        /// <summary>
        /// Đánh giá sức khỏe dựa trên các chỉ số cơ thể.
        /// Reimplementation of health_assessor.py (testrcm) trong C#.
        /// </summary>
        (HealthAssessResultDto? result, string? error) Assess(HealthAssessRequestDto request);
    }
}
