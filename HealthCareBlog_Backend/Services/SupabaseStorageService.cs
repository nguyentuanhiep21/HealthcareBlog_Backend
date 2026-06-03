using HealthCareBlog_Backend.Services.Interfaces;
using Supabase;

namespace HealthCareBlog_Backend.Services
{
    public class SupabaseStorageService : ISupabaseStorageService
    {
        private readonly Client _supabaseClient;
        private readonly IConfiguration _configuration;

        public SupabaseStorageService(Client supabaseClient, IConfiguration configuration)
        {
            _supabaseClient = supabaseClient;
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var bucketName = _configuration["Supabase:Bucket"] ?? "HealthcareBlog_Image";
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var storagePath = string.IsNullOrEmpty(folderName) ? uniqueFileName : $"{folderName}/{uniqueFileName}";

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Upload the file to the bucket
            await _supabaseClient.Storage
                .From(bucketName)
                .Upload(fileBytes, storagePath, new Supabase.Storage.FileOptions { CacheControl = "3600", Upsert = false });

            // Get public URL
            var publicUrl = _supabaseClient.Storage.From(bucketName).GetPublicUrl(storagePath);
            return publicUrl;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                var bucketName = _configuration["Supabase:Bucket"] ?? "HealthcareBlog_Image";

                // Extract storage path from public URL
                // Public URL format: https://<project>.supabase.co/storage/v1/object/public/<bucket>/<path>
                var marker = $"/object/public/{bucketName}/";
                var idx = fileUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                {
                    Console.WriteLine($"[SupabaseStorageService] Could not extract path from URL: {fileUrl}");
                    return;
                }

                var storagePath = fileUrl[(idx + marker.Length)..];
                await _supabaseClient.Storage
                    .From(bucketName)
                    .Remove(new List<string> { storagePath });

                Console.WriteLine($"[SupabaseStorageService] Deleted: {storagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SupabaseStorageService] DeleteFileAsync error: {ex.Message}");
            }
        }
    }
}
