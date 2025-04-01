namespace ScpmaBe.Services.Interfaces
{
    public interface IImageService
    {
        Task<bool> UploadImageAsync(string path, Stream fsStream);
    }
}
