using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthCareBlog_Backend.Services.Interfaces;

namespace HealthCareBlog_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ISupabaseStorageService _supabaseStorageService;

        public UploadController(ISupabaseStorageService supabaseStorageService)
        {
            _supabaseStorageService = supabaseStorageService;
        }

        [HttpPost("image")]
        [Authorize]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Vui lòng chọn ảnh để upload.", success = false });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Chỉ hỗ trợ file ảnh (.jpg, .jpeg, .png, .gif, .webp).", success = false });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Kích thước ảnh không được vượt quá 5MB.", success = false });
                }

                // Upload to Supabase Storage
                var fileUrl = await _supabaseStorageService.UploadFileAsync(file, "Posts");

                // Return URL
                return Ok(new { url = fileUrl, success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UploadController] Error: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi upload ảnh.", success = false });
            }
        }

        [HttpPost("avatar")]
        [Authorize]
        public async Task<ActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Vui lòng chọn ảnh để upload.", success = false });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Chỉ hỗ trợ file ảnh (.jpg, .jpeg, .png, .gif, .webp).", success = false });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Kích thước ảnh không được vượt quá 5MB.", success = false });
                }

                // Upload to Supabase Storage
                var fileUrl = await _supabaseStorageService.UploadFileAsync(file, "Avatars");

                // Return URL
                return Ok(new { url = fileUrl, success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UploadController] Error uploading avatar: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi upload avatar.", success = false });
            }
        }
    }
}
