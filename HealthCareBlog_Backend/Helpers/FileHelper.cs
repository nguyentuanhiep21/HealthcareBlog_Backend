namespace HealthCareBlog_Backend.Helpers
{
    /// <summary>
    /// Helper class for file operations (upload, delete)
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Delete a file from wwwroot folder
        /// </summary>
        /// <param name="fileUrl">Relative URL of file (e.g., /uploads/posts/image.jpg)</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public static bool DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return false;

            try
            {
                var filePath = fileUrl.TrimStart('/');
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"[FileHelper] Deleted file: {fullPath}");
                    return true;
                }
                
                Console.WriteLine($"[FileHelper] File not found: {fullPath}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileHelper] Error deleting file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete avatar file (with additional checks for default avatar)
        /// </summary>
        /// <param name="avatarUrl">Avatar URL</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public static bool DeleteAvatar(string avatarUrl)
        {
            // Don't delete default avatars
            if (string.IsNullOrEmpty(avatarUrl) || 
                avatarUrl == "/images/logo.png" || 
                !avatarUrl.StartsWith("/uploads/avatars/"))
            {
                return false;
            }

            return DeleteFile(avatarUrl);
        }

        /// <summary>
        /// Delete banner file
        /// </summary>
        /// <param name="bannerUrl">Banner URL</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public static bool DeleteBanner(string bannerUrl)
        {
            if (string.IsNullOrEmpty(bannerUrl) || !bannerUrl.StartsWith("/uploads/banners/"))
            {
                return false;
            }

            return DeleteFile(bannerUrl);
        }

        /// <summary>
        /// Delete post image file
        /// </summary>
        /// <param name="imageUrl">Image URL</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public static bool DeletePostImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/uploads/posts/"))
            {
                return false;
            }

            return DeleteFile(imageUrl);
        }
    }
}
