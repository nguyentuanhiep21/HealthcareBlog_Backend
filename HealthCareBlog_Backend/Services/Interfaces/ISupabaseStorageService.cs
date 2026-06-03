using Microsoft.AspNetCore.Http;

namespace HealthCareBlog_Backend.Services.Interfaces
{
    public interface ISupabaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string fileUrl);
    }
}
