using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class BannerAdminService : IBannerAdminService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BannerAdminService(
            WebsiteBanMyPhamContext context,
            IFileUploadService fileUploadService,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _localizer = localizer;
        }

        public async Task<List<BannerAdminListItemViewModel>> LayDanhSachAsync()
        {
            return await _context.Banner
                .AsNoTracking()
                .OrderBy(b => b.ThuTu)
                .ThenBy(b => b.MaBanner)
                .Select(b => new BannerAdminListItemViewModel
                {
                    MaBanner = b.MaBanner,
                    TieuDe = b.TieuDe,
                    DuongDanAnh = b.DuongDanAnh,
                    LienKet = b.LienKet,
                    ThuTu = b.ThuTu,
                    TrangThai = b.TrangThai
                })
                .ToListAsync();
        }

        public async Task<BannerAdminFormViewModel?> LayDeSuaAsync(int maBanner)
        {
            var b = await _context.Banner.AsNoTracking().FirstOrDefaultAsync(x => x.MaBanner == maBanner);
            if (b == null) return null;

            return new BannerAdminFormViewModel
            {
                MaBanner = b.MaBanner,
                TieuDe = b.TieuDe,
                DuongDanAnh = b.DuongDanAnh,
                LienKet = b.LienKet,
                ThuTu = b.ThuTu,
                TrangThai = b.TrangThai
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? MaBanner)> ThemBannerAsync(BannerAdminFormViewModel model)
        {
            if (model.FileAnh == null || model.FileAnh.Length == 0)
            {
                return (false, _localizer["Admin_Banner_ImageRequired"].Value, null);
            }

            var (validLink, normalizedLink, linkError) = KiemTraVaChuanHoaLienKet(model.LienKet);
            if (!validLink)
            {
                return (false, linkError, null);
            }

            var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(model.FileAnh, UploadFolderConstant.Banners);
            if (!uploadSuccess || string.IsNullOrEmpty(filePath))
            {
                return (false, uploadError ?? _localizer["Upload_Error_System"].Value, null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var banner = new Banner
                {
                    TieuDe = string.IsNullOrWhiteSpace(model.TieuDe) ? null : model.TieuDe.Trim(),
                    DuongDanAnh = filePath,
                    LienKet = normalizedLink,
                    ThuTu = Math.Max(0, model.ThuTu),
                    TrangThai = model.TrangThai
                };

                _context.Banner.Add(banner);
                await _context.SaveChangesAsync();

                // HasDefaultValue(true): Nếu tạo với TrangThai = false thì update trong transaction
                if (!model.TrangThai)
                {
                    await _context.Banner
                        .Where(b => b.MaBanner == banner.MaBanner)
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.TrangThai, false));
                    banner.TrangThai = false;
                }

                await transaction.CommitAsync();
                return (true, null, banner.MaBanner);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _fileUploadService.DeleteFile(filePath, UploadFolderConstant.Banners);
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatBannerAsync(int maBanner, BannerAdminFormViewModel model)
        {
            var banner = await _context.Banner.FindAsync(maBanner);
            if (banner == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            var (validLink, normalizedLink, linkError) = KiemTraVaChuanHoaLienKet(model.LienKet);
            if (!validLink)
            {
                return (false, linkError);
            }

            string? newFilePath = null;
            string? oldFilePath = null;

            if (model.FileAnh != null && model.FileAnh.Length > 0)
            {
                var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(model.FileAnh, UploadFolderConstant.Banners);
                if (!uploadSuccess || string.IsNullOrEmpty(filePath))
                {
                    return (false, uploadError ?? _localizer["Upload_Error_System"].Value);
                }

                newFilePath = filePath;
                oldFilePath = banner.DuongDanAnh;
                banner.DuongDanAnh = newFilePath;
            }

            banner.TieuDe = string.IsNullOrWhiteSpace(model.TieuDe) ? null : model.TieuDe.Trim();
            banner.LienKet = normalizedLink;
            banner.ThuTu = Math.Max(0, model.ThuTu);
            banner.TrangThai = model.TrangThai;

            try
            {
                await _context.SaveChangesAsync();

                // Lưu database thành công -> Xóa file ảnh cũ nếu có thay ảnh
                if (!string.IsNullOrEmpty(oldFilePath))
                {
                    _fileUploadService.DeleteFile(oldFilePath, UploadFolderConstant.Banners);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                // Lưu thất bại -> Xóa file ảnh mới vừa tải lên nếu có
                if (!string.IsNullOrEmpty(newFilePath))
                {
                    _fileUploadService.DeleteFile(newFilePath, UploadFolderConstant.Banners);
                }

                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaBannerAsync(int maBanner)
        {
            var banner = await _context.Banner.FindAsync(maBanner);
            if (banner == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            var oldPath = banner.DuongDanAnh;
            try
            {
                _context.Banner.Remove(banner);
                await _context.SaveChangesAsync();

                _fileUploadService.DeleteFile(oldPath, UploadFolderConstant.Banners);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DatTrangThaiBannerAsync(int maBanner, bool trangThaiMoi)
        {
            var banner = await _context.Banner.FindAsync(maBanner);
            if (banner == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            banner.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatThuTuAsync(int maBanner, int thuTuMoi)
        {
            var banner = await _context.Banner.FindAsync(maBanner);
            if (banner == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            banner.ThuTu = Math.Max(0, thuTuMoi);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Kiểm tra và chuẩn hóa liên kết theo Điều chỉnh 1:
        /// - Trim trước khi kiểm tra
        /// - Chuỗi rỗng sau Trim lưu NULL
        /// - Chặn "//"
        /// - Chặn mọi đường dẫn nội bộ chứa dấu "\" (ví dụ "/\evil.com")
        /// - Chặn ký tự điều khiển
        /// - Chấp nhận: đường dẫn nội bộ bắt đầu bằng "/" hoặc URL tuyệt đối http/https
        /// </summary>
        private (bool Valid, string? Normalized, string? ErrorMessage) KiemTraVaChuanHoaLienKet(string? rawLink)
        {
            if (string.IsNullOrWhiteSpace(rawLink))
            {
                return (true, null, null);
            }

            var link = rawLink.Trim();
            if (string.IsNullOrEmpty(link))
            {
                return (true, null, null);
            }

            if (link.Length > 255)
            {
                return (false, null, _localizer["Validation_MaxLength", 255].Value);
            }

            // Chặn ký tự điều khiển
            if (link.Any(char.IsControl))
            {
                return (false, null, _localizer["Admin_Banner_LinkInvalid"].Value);
            }

            // Đường dẫn nội bộ
            if (link.StartsWith("/"))
            {
                // Chặn "//" và chặn "\" (đặc biệt "/\evil.com")
                if (link.StartsWith("//") || link.Contains('\\'))
                {
                    return (false, null, _localizer["Admin_Banner_LinkInvalid"].Value);
                }

                return (true, link, null);
            }

            // URL tuyệt đối (http / https)
            if (Uri.TryCreate(link, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return (true, link, null);
            }

            return (false, null, _localizer["Admin_Banner_LinkInvalid"].Value);
        }
    }
}

