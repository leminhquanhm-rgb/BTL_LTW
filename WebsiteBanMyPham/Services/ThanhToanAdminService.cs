using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class ThanhToanAdminService : IThanhToanAdminService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<ThanhToanAdminService> _logger;

        public ThanhToanAdminService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer,
            ILogger<ThanhToanAdminService> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<PaginatedList<ThanhToanAdminListItemViewModel>> LayDanhSachGiaoDichAsync(ThanhToanAdminFilterViewModel filter)
        {
            int validTrang = filter.Trang < 1 ? 1 : filter.Trang;
            int validPageSize = Math.Clamp(filter.PageSize, 1, 50);

            var query = _context.ThanhToan
                .AsNoTracking()
                .Include(t => t.MaDHNavigation)
                    .ThenInclude(d => d.MaTKNavigation)
                .Include(t => t.MaPTNavigation)
                .AsQueryable();

            if (filter.MaPT.HasValue && filter.MaPT.Value > 0)
            {
                query = query.Where(t => t.MaPT == filter.MaPT.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            {
                var st = filter.TrangThai.Trim();
                query = query.Where(t => t.TrangThai == st);
            }

            if (!string.IsNullOrWhiteSpace(filter.TrangThaiHoanTien))
            {
                var rf = filter.TrangThaiHoanTien.Trim();
                query = query.Where(t => t.TrangThaiHoanTien == rf);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                var kwLower = kw.ToLower();
                var maDHParsed = DonHangQueryExtensions.BocTachMaDonHang(kw);

                if (maDHParsed.HasValue)
                {
                    query = query.Where(t => t.MaDH == maDHParsed.Value
                                          || (t.MaGiaoDich != null && t.MaGiaoDich.ToLower().Contains(kwLower))
                                          || (t.MaDHNavigation.MaTKNavigation != null && t.MaDHNavigation.MaTKNavigation.HoTen.ToLower().Contains(kwLower)));
                }
                else
                {
                    query = query.Where(t => (t.MaGiaoDich != null && t.MaGiaoDich.ToLower().Contains(kwLower))
                                          || (t.MaDHNavigation.MaTKNavigation != null && t.MaDHNavigation.MaTKNavigation.HoTen.ToLower().Contains(kwLower)));
                }
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.NgayTao)
                .ThenByDescending(t => t.MaTT)
                .Skip((validTrang - 1) * validPageSize)
                .Take(validPageSize)
                .Select(t => new ThanhToanAdminListItemViewModel
                {
                    MaTT = t.MaTT,
                    MaDH = t.MaDH,
                    TenKhachHang = t.MaDHNavigation.MaTKNavigation != null ? t.MaDHNavigation.MaTKNavigation.HoTen : "",
                    EmailKhachHang = t.MaDHNavigation.MaTKNavigation != null ? t.MaDHNavigation.MaTKNavigation.Email : "",
                    MaPT = t.MaPT,
                    TenPT = t.MaPTNavigation.TenPT,
                    SoTien = t.SoTien,
                    TrangThai = t.TrangThai,
                    TrangThaiHoanTien = t.TrangThaiHoanTien,
                    MaGiaoDich = t.MaGiaoDich,
                    NgayThanhToan = t.NgayThanhToan,
                    NgayHoanTien = t.NgayHoanTien,
                    NgayTao = t.NgayTao
                })
                .ToListAsync();

            return new PaginatedList<ThanhToanAdminListItemViewModel>(items, totalItems, validTrang, validPageSize);
        }

        public async Task<ThanhToanAdminOverviewViewModel> LayTongQuanGiaoDichAsync()
        {
            var data = await _context.ThanhToan
                .AsNoTracking()
                .Select(t => new { t.TrangThai, t.TrangThaiHoanTien })
                .ToListAsync();

            return new ThanhToanAdminOverviewViewModel
            {
                TongSoGiaoDich = data.Count,
                SoGiaoDichChoHoan = data.Count(t => t.TrangThai == TrangThaiThanhToanConstant.DaThanhToan && t.TrangThaiHoanTien == TrangThaiHoanTienConstant.ChoHoan),
                SoGiaoDichDaThanhToan = data.Count(t => t.TrangThai == TrangThaiThanhToanConstant.DaThanhToan),
                SoGiaoDichChuaThanhToan = data.Count(t => t.TrangThai == TrangThaiThanhToanConstant.ChuaThanhToan)
            };
        }

        public async Task<List<PhuongThucThanhToanAdminViewModel>> LayDanhSachPhuongThucAsync()
        {
            return await _context.PhuongThucThanhToan
                .AsNoTracking()
                .OrderBy(p => p.MaPT)
                .Select(p => new PhuongThucThanhToanAdminViewModel
                {
                    MaPT = p.MaPT,
                    TenPT = p.TenPT,
                    MoTa = p.MoTa,
                    TrangThai = p.TrangThai
                })
                .ToListAsync();
        }

        public async Task<PhuongThucThanhToanAdminViewModel?> LayChiTietPhuongThucAsync(int maPT)
        {
            return await _context.PhuongThucThanhToan
                .AsNoTracking()
                .Where(p => p.MaPT == maPT)
                .Select(p => new PhuongThucThanhToanAdminViewModel
                {
                    MaPT = p.MaPT,
                    TenPT = p.TenPT,
                    MoTa = p.MoTa,
                    TrangThai = p.TrangThai
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatPhuongThucAsync(int maPT, string? moTa)
        {
            var pt = await _context.PhuongThucThanhToan.FirstOrDefaultAsync(p => p.MaPT == maPT);
            if (pt == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            var trimmed = moTa?.Trim();
            if (trimmed != null && trimmed.Length > 255)
            {
                return (false, _localizer["Validation_MaxLength", _localizer["Admin_PaymentMethod_Desc"].Value, 255].Value);
            }

            try
            {
                pt.MoTa = string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mô tả phương thức thanh toán MaPT {MaPT}", maPT);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DatTrangThaiPhuongThucAsync(int maPT, bool trangThaiMoi)
        {
            var pt = await _context.PhuongThucThanhToan.FirstOrDefaultAsync(p => p.MaPT == maPT);
            if (pt == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            // Chặn đặt COD = false ở cả client và server theo Điều chỉnh 3
            if (pt.TenPT == PhuongThucThanhToanConstant.COD && !trangThaiMoi)
            {
                return (false, _localizer["Admin_PaymentMethod_CodCannotDisable"].Value);
            }

            try
            {
                pt.TrangThai = trangThaiMoi;
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt trạng thái phương thức thanh toán MaPT {MaPT} thành {TrangThaiMoi}", maPT, trangThaiMoi);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }
    }
}

