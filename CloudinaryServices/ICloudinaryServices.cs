namespace BackendProject2.CloudinaryServices
{
    public interface ICloudinaryServices
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
