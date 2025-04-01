using ScpmaBe.Services.Interfaces;

namespace ScpmaBe.Services
{
    public class ImageService : IImageService
    {
        public async Task<bool> UploadImageAsync(string path, Stream fsStream)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || fsStream == null || !fsStream.CanRead)
                {
                    return false;
                }

                // Ensure directory exists
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write the file
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    await fsStream.CopyToAsync(fileStream);
                }

                return true;
            }
            catch (Exception)
            {
                // Log exception if needed
                return false;
            }
        }
    }
}
