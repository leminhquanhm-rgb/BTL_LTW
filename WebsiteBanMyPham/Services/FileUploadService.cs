using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.Services
{
    public class FileUploadService : IFileUploadService
    {
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };
        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/pjpeg", "image/png", "image/x-png", "image/webp"
        };

        private readonly IWebHostEnvironment _environment;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(
            IWebHostEnvironment environment,
            IStringLocalizer<SharedResource> localizer,
            ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _localizer = localizer;
            _logger = logger;
        }

        private string GetUploadsRoot()
        {
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }
            return Path.GetFullPath(Path.Combine(webRoot, "uploads"));
        }

        private static bool IsSubdirectory(string candidatePath, string parentDirectory)
        {
            var normalizedParent = Path.GetFullPath(parentDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var normalizedCandidate = Path.GetFullPath(candidatePath);

            return normalizedCandidate.StartsWith(normalizedParent, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<(bool Success, string? FilePath, LoaiLoiUpload? ErrorType, string? ErrorMessage)> UploadImageAsync(
            IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, LoaiLoiUpload.KhongCoFile, _localizer["Upload_Error_NoFile"].Value);
            }

            if (file.Length > MaxFileSize)
            {
                return (false, null, LoaiLoiUpload.FileQuaLon, _localizer["Upload_Error_FileTooLarge"].Value);
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                return (false, null, LoaiLoiUpload.DinhDangKhongHopLe, _localizer["Upload_Error_InvalidExtension"].Value);
            }

            if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedMimeTypes.Contains(file.ContentType))
            {
                return (false, null, LoaiLoiUpload.DinhDangKhongHopLe, _localizer["Upload_Error_InvalidMimeType"].Value);
            }

            // Kiểm tra magic bytes
            try
            {
                using var stream = file.OpenReadStream();
                var headerBytes = new byte[12];
                int bytesRead = await stream.ReadAsync(headerBytes, 0, headerBytes.Length);

                if (!KiemTraMagicBytes(headerBytes, bytesRead, extension))
                {
                    return (false, null, LoaiLoiUpload.DinhDangKhongHopLe, _localizer["Upload_Error_InvalidMagicBytes"].Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đọc file upload để kiểm tra magic bytes: {FileName}", file.FileName);
                return (false, null, LoaiLoiUpload.LoiHeThong, _localizer["Upload_Error_System"].Value);
            }

            try
            {
                var cleanFolder = Path.GetFileName(folder.Trim().Replace('\\', '/').Trim('/'));
                if (string.IsNullOrWhiteSpace(cleanFolder))
                {
                    cleanFolder = "misc";
                }

                var uploadsRoot = GetUploadsRoot();
                var targetDir = Path.GetFullPath(Path.Combine(uploadsRoot, cleanFolder));

                if (!IsSubdirectory(targetDir, uploadsRoot))
                {
                    _logger.LogWarning("Phát hiện nguy cơ path traversal khi upload vào thư mục {Folder}", folder);
                    return (false, null, LoaiLoiUpload.LoiHeThong, _localizer["Upload_Error_System"].Value);
                }

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var uniqueFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
                var destinationPath = Path.Combine(targetDir, uniqueFileName);

                using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var relativeUrl = $"/uploads/{cleanFolder}/{uniqueFileName}";
                return (true, relativeUrl, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lưu file upload {FileName}", file.FileName);
                return (false, null, LoaiLoiUpload.LoiHeThong, _localizer["Upload_Error_System"].Value);
            }
        }

        public bool DeleteFile(string? relativePath, string folder)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(folder)) return false;

            try
            {
                var cleanFolder = Path.GetFileName(folder.Trim().Replace('\\', '/').Trim('/'));
                if (string.IsNullOrWhiteSpace(cleanFolder)) return false;

                var normalized = relativePath.Trim().Replace('\\', '/');
                var expectedPrefix = $"/uploads/{cleanFolder}/";
                var expectedPrefixAlt = $"uploads/{cleanFolder}/";

                if (!normalized.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase) &&
                    !normalized.StartsWith(expectedPrefixAlt, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Đường dẫn file {Path} không thuộc thư mục uploads/{Folder}/", relativePath, cleanFolder);
                    return false;
                }

                var trimmed = normalized.TrimStart('/');
                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
                }

                var uploadsRoot = GetUploadsRoot();
                var folderRoot = Path.GetFullPath(Path.Combine(uploadsRoot, cleanFolder));
                var fullPath = Path.GetFullPath(Path.Combine(webRoot, trimmed.Replace('/', Path.DirectorySeparatorChar)));

                if (!IsSubdirectory(fullPath, folderRoot))
                {
                    _logger.LogWarning("Phát hiện nguy cơ path traversal khi xóa file: {Path} trong thư mục {Folder}", relativePath, cleanFolder);
                    return false;
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể xóa file tại đường dẫn: {Path}", relativePath);
            }

            return false;
        }

        private static bool KiemTraMagicBytes(byte[] bytes, int length, string extension)
        {
            if (length < 3) return false;

            var ext = extension.ToLowerInvariant();

            if (ext is ".jpg" or ".jpeg")
            {
                // JPEG: FF D8 FF
                return bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
            }

            if (ext == ".png")
            {
                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (length < 8) return false;
                return bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
                       bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
            }

            if (ext == ".webp")
            {
                // WebP: RIFF (bytes 0-3) and WEBP (bytes 8-11)
                if (length < 12) return false;
                bool isRiff = bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46;
                bool isWebp = bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50;
                return isRiff && isWebp;
            }

            return false;
        }
    }
}
