using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class TaiKhoanAdminService : ITaiKhoanAdminService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IMemoryCache _cache;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TaiKhoanAdminService(
            WebsiteBanMyPhamContext context,
            IMemoryCache cache,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _cache = cache;
            _localizer = localizer;
        }

        public async Task<PaginatedList<KhachHangAdminListItemViewModel>> LayDanhSachKhachHangAsync(KhachHangAdminFilterViewModel filter)
        {
            // CHỈ lấy tài khoản khách hàng, loại bỏ tuyệt đối QuanTriVien
            var query = _context.TaiKhoan
                .AsNoTracking()
                .Where(t => t.VaiTro == VaiTroConstant.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(t => t.HoTen.Contains(kw) ||
                                         t.Email.Contains(kw) ||
                                         (t.SoDienThoai != null && t.SoDienThoai.Contains(kw)));
            }

            if (filter.TrangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == filter.TrangThai.Value);
            }

            query = query.OrderByDescending(t => t.NgayTao).ThenByDescending(t => t.MaTK);

            int totalItems = await query.CountAsync();
            int validPage = Math.Max(1, filter.Trang);
            int validSize = Math.Clamp(filter.PageSize, 1, 50);

            var items = await query
                .Skip((validPage - 1) * validSize)
                .Take(validSize)
                .Select(t => new KhachHangAdminListItemViewModel
                {
                    MaTK = t.MaTK,
                    HoTen = t.HoTen,
                    Email = t.Email,
                    SoDienThoai = t.SoDienThoai,
                    NgayTao = t.NgayTao,
                    SoDonHang = _context.DonHang.Count(d => d.MaTK == t.MaTK),
                    TongChiTieu = _context.DonHang
                        .Where(d => d.MaTK == t.MaTK && d.TrangThai == TrangThaiDonHangConstant.HoanThanh)
                        .Sum(d => (decimal?)d.TongThanhToan) ?? 0,
                    TrangThai = t.TrangThai
                })
                .ToListAsync();

            return new PaginatedList<KhachHangAdminListItemViewModel>(items, totalItems, validPage, validSize);
        }

        public async Task<KhachHangAdminDetailViewModel?> LayChiTietKhachHangAsync(int maTK)
        {
            var tk = await _context.TaiKhoan
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaTK == maTK && t.VaiTro == VaiTroConstant.KhachHang);

            if (tk == null) return null;

            int tongSoDon = await _context.DonHang.CountAsync(d => d.MaTK == maTK);
            int soDonHoanThanh = await _context.DonHang.CountAsync(d => d.MaTK == maTK && d.TrangThai == TrangThaiDonHangConstant.HoanThanh);
            decimal tongChiTieu = await _context.DonHang
                .Where(d => d.MaTK == maTK && d.TrangThai == TrangThaiDonHangConstant.HoanThanh)
                .SumAsync(d => (decimal?)d.TongThanhToan) ?? 0;

            var recentOrders = await _context.DonHang
                .AsNoTracking()
                .Where(d => d.MaTK == maTK)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .Select(d => new KhachHangDonHangGanNhatViewModel
                {
                    MaDH = d.MaDH,
                    NgayDat = d.NgayDat,
                    TongThanhToan = d.TongThanhToan,
                    TrangThai = d.TrangThai,
                    TrangThaiThanhToan = d.ThanhToan != null ? d.ThanhToan.TrangThai : null,
                    TrangThaiHoanTien = d.ThanhToan != null ? d.ThanhToan.TrangThaiHoanTien : null
                })
                .ToListAsync();

            return new KhachHangAdminDetailViewModel
            {
                MaTK = tk.MaTK,
                HoTen = tk.HoTen,
                Email = tk.Email,
                SoDienThoai = tk.SoDienThoai,
                DiaChi = tk.DiaChi,
                NgayTao = tk.NgayTao,
                TrangThai = tk.TrangThai,
                TongSoDon = tongSoDon,
                SoDonHoanThanh = soDonHoanThanh,
                TongChiTieu = tongChiTieu,
                DonHangGanNhat = recentOrders
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> DatTrangThaiKhachHangAsync(int maTK, bool trangThaiMoi)
        {
            var tk = await _context.TaiKhoan.FindAsync(maTK);
            if (tk == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            // Chặn tuyệt đối không cho tác động lên tài khoản Admin
            if (tk.VaiTro != VaiTroConstant.KhachHang)
            {
                return (false, _localizer["Admin_Customer_Error_CannotModifyAdmin"].Value);
            }

            tk.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();

            // Xóa cache xác thực tài khoản để CookieValidator tự động đăng xuất khách hàng ở request kế tiếp
            CookieValidator.XoaCacheTaiKhoan(_cache, maTK);

            return (true, null);
        }
    }
}

