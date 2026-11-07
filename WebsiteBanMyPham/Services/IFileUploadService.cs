using Microsoft.AspNetCore.Http;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.Services
{
    public interface IFileUploadService
    {
        Task<(bool Success, string? FilePath, LoaiLoiUpload? ErrorType, string? ErrorMessage)> UploadImageAsync(IFormFile? file, string folder);
        bool DeleteFile(string? relativePath, string folder);
    }
}

