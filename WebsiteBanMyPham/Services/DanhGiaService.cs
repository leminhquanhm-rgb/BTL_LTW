using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<DanhGiaService> _logger;

        public DanhGiaService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer,
            ILogger<DanhGiaService> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<ThongTinDanhGiaViewModel?> LayThongTinDanhGiaAsync(int maTK, int maCTDH)
        {
            var ct = await _context.ChiTietDonHang
                .AsNoTracking()
                .Where(c => c.MaCTDH == maCTDH)
                .Include(c => c.MaDHNavigation)
                .Include(c => c.MaSPNavigation)
                    .ThenInclude(s => s.AnhSanPham)
                .Include(c => c.DanhGia)
                .FirstOrDefaultAsync();

            if (ct == null || ct.MaDHNavigation.MaTK != maTK)
            {
                return null;
            }

            bool daDanhGia = ct.DanhGia != null;
            bool donHoanThanh = ct.MaDHNavigation.TrangThai == TrangThaiDonHangConstant.HoanThanh;
            bool coTheDanhGia = donHoanThanh && !daDanhGia;

            string? lyDo = null;
            if (daDanhGia)
            {
                lyDo = _localizer["Review_Error_AlreadyReviewed"].Value;
            }
            else if (!donHoanThanh)
            {
                lyDo = _localizer["Review_Error_NotCompleted"].Value;
            }

            var anh = ct.MaSPNavigation.AnhSanPham
                .OrderByDescending(a => a.LaAnhChinh)
                .ThenBy(a => a.MaAnh)
                .FirstOrDefault()?.DuongDan;

            return new ThongTinDanhGiaViewModel
            {
                MaCTDH = ct.MaCTDH,
                MaDH = ct.MaDH,
                MaSP = ct.MaSP,
                TenSP = ct.TenSP,
                DuongDanAnh = anh,
                DonGia = ct.DonGia,
                SoLuong = ct.SoLuong,
                NgayGiaoThanhCong = ct.MaDHNavigation.NgayGiaoThanhCong,
                CoTheDanhGia = coTheDanhGia,
                LyDoKhongTheDanhGia = lyDo
            };
        }

        public async Task<(bool Success, string? ErrorMessage, LoaiLoiDanhGia LoaiLoi)> TaoDanhGiaAsync(
            int maTK, int maCTDH, byte soSao, string? noiDung)
        {
            if (soSao < 1 || soSao > 5)
            {
                return (false, _localizer["Review_Error_InvalidStar"].Value, LoaiLoiDanhGia.DuLieuKhongHopLe);
            }

            string? ndClean = string.IsNullOrWhiteSpace(noiDung) ? null : noiDung.Trim();
            if (ndClean != null && ndClean.Length > 1000)
            {
                return (false, _localizer["Review_Error_ContentTooLong"].Value, LoaiLoiDanhGia.DuLieuKhongHopLe);
            }

            var ct = await _context.ChiTietDonHang
                .Include(c => c.MaDHNavigation)
                .Include(c => c.DanhGia)
                .FirstOrDefaultAsync(c => c.MaCTDH == maCTDH);

            if (ct == null || ct.MaDHNavigation.MaTK != maTK)
            {
                return (false, _localizer["Review_Error_NotFoundOrNotOwned"].Value, LoaiLoiDanhGia.KhongTimThay);
            }

            if (ct.MaDHNavigation.TrangThai != TrangThaiDonHangConstant.HoanThanh)
            {
                return (false, _localizer["Review_Error_NotCompleted"].Value, LoaiLoiDanhGia.XungDot);
            }

            if (ct.DanhGia != null)
            {
                return (false, _localizer["Review_Error_AlreadyReviewed"].Value, LoaiLoiDanhGia.XungDot);
            }

            var danhGia = new DanhGia
            {
                MaCTDH = maCTDH,
                SoSao = soSao,
                NoiDung = ndClean,
                TrangThai = TrangThaiDanhGiaConstant.ChoDuyet,
                NgayTao = DateTime.Now
            };

            _context.DanhGia.Add(danhGia);

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, LoaiLoiDanhGia.KhongCoLoi);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "DbUpdateException khi tạo đánh giá cho MaCTDH {MaCTDH} bởi MaTK {MaTK}", maCTDH, maTK);
                return (false, _localizer["Review_Error_AlreadyReviewed"].Value, LoaiLoiDanhGia.XungDot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ngoại lệ khi tạo đánh giá cho MaCTDH {MaCTDH} bởi MaTK {MaTK}", maCTDH, maTK);
                return (false, _localizer["Common_Error"].Value, LoaiLoiDanhGia.DuLieuKhongHopLe);
            }
        }
    }
}

