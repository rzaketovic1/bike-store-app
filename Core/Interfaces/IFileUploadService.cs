using Microsoft.AspNetCore.Http;

namespace Core.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "products");
        bool DeleteImage(string fileName, string folder = "products");
        bool IsValidImage(IFormFile file);
    }
}
