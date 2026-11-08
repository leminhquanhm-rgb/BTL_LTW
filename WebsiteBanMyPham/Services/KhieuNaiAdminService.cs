using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class KhieuNaiAdminService : IKhieuNaiAdminService
    {
        private const int PageSizeFixed = 10;
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhieuNaiAdminService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<PaginatedList<KhieuNaiAdminListItemViewModel>> LayDanhSachAsync(KhieuNaiAdminFilterViewModel filter)
        {
            int page = filter.Trang < 1 ? 1 : filter.Trang;

            // Chuẩn hóa trạng thái: thiếu hoặc không hợp lệ -> DangMo
            string trangThaiChuan = ChuanHoaTrangThai(filter.TrangThai);
            filter.TrangThai = trangThaiChuan;

            var query = _context.KhieuNai.AsNoTracking();

            if (trangThaiChuan == BoLocTrangThaiConstant.DangMo)
            {
                query = query.Where(k => k.TrangThai == TrangThaiKhieuNaiConstant.ChoXuLy || k.TrangThai == TrangThaiKhieuNaiConstant.DangXuLy);
            }
            else if (trangThaiChuan != BoLocTrangThaiConstant.TatCa)
            {
                query = query.Where(k => k.TrangThai == trangThaiChuan);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                var parsedMaDH = DonHangQueryExtensions.BocTachMaDonHang(kw);

                if (parsedMaDH.HasValue)
                {
                    query = query.Where(k =>
                        k.MaDH == parsedMaDH.Value ||
                        k.MaTKNavigation.HoTen.Contains(kw) ||
                        k.MaTKNavigation.Email.Contains(kw));
                }
                else
                {
                    query = query.Where(k =>
                        k.MaTKNavigation.HoTen.Contains(kw) ||
                        k.MaTKNavigation.Email.Contains(kw));
                }
            }

            int totalItems = await query.CountAsync();

            // Sắp xếp: khiếu nại đang mở (DangMo, ChoXuLy, DangXuLy) cũ nhất trước; đã đóng (DaGiaiQuyet, TuChoi) hoặc tất cả mới nhất trước
            bool laDangMo = trangThaiChuan == BoLocTrangThaiConstant.DangMo ||
                            trangThaiChuan == TrangThaiKhieuNaiConstant.ChoXuLy ||
                            trangThaiChuan == TrangThaiKhieuNaiConstant.DangXuLy;

            var orderedQuery = laDangMo
                ? query.OrderBy(k => k.NgayGui)
                : query.OrderByDescending(k => k.NgayGui);

            var items = await orderedQuery
                .Skip((page - 1) * PageSizeFixed)
                .Take(PageSizeFixed)
                .Select(k => new KhieuNaiAdminListItemViewModel
                {
                    MaKN = k.MaKN,
                    MaTK = k.MaTK,
                    TenKhachHang = k.MaTKNavigation.HoTen,
                    EmailKhachHang = k.MaTKNavigation.Email,
                    MaDH = k.MaDH,
                    MaSP = k.MaSP,
                    TenSP = k.MaSPNavigation != null ? k.MaSPNavigation.TenSP : null,
                    NoiDung = k.NoiDung,
                    NgayGui = k.NgayGui,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();

            return new PaginatedList<KhieuNaiAdminListItemViewModel>(items, totalItems, page, PageSizeFixed);
        }

        public async Task<KhieuNaiAdminStatusCountViewModel> LaySoLuongTheoTrangThaiAsync(string? keyword)
        {
            var query = _context.KhieuNai.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                var parsedMaDH = DonHangQueryExtensions.BocTachMaDonHang(kw);

                if (parsedMaDH.HasValue)
                {
                    query = query.Where(k =>
                        k.MaDH == parsedMaDH.Value ||
                        k.MaTKNavigation.HoTen.Contains(kw) ||
                        k.MaTKNavigation.Email.Contains(kw));
                }
                else
                {
                    query = query.Where(k =>
                        k.MaTKNavigation.HoTen.Contains(kw) ||
                        k.MaTKNavigation.Email.Contains(kw));
                }
            }

            var counts = await query
                .GroupBy(k => k.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToListAsync();

            int choXuLy = counts.FirstOrDefault(c => c.TrangThai == TrangThaiKhieuNaiConstant.ChoXuLy)?.Count ?? 0;
            int dangXuLy = counts.FirstOrDefault(c => c.TrangThai == TrangThaiKhieuNaiConstant.DangXuLy)?.Count ?? 0;
            int daGiaiQuyet = counts.FirstOrDefault(c => c.TrangThai == TrangThaiKhieuNaiConstant.DaGiaiQuyet)?.Count ?? 0;
            int tuChoi = counts.FirstOrDefault(c => c.TrangThai == TrangThaiKhieuNaiConstant.TuChoi)?.Count ?? 0;

            return new KhieuNaiAdminStatusCountViewModel
            {
                DangMo = choXuLy + dangXuLy,
                ChoXuLy = choXuLy,
                DangXuLy = dangXuLy,
                DaGiaiQuyet = daGiaiQuyet,
                TuChoi = tuChoi,
                TatCa = choXuLy + dangXuLy + daGiaiQuyet + tuChoi
            };
        }

        public async Task<KhieuNaiAdminDetailViewModel?> LayChiTietAsync(int id)
        {
            if (id <= 0) return null;

            var kn = await _context.KhieuNai
                .AsNoTracking()
                .Include(k => k.MaTKNavigation)
                .Include(k => k.MaDHNavigation)
                .Include(k => k.MaSPNavigation)
                    .ThenInclude(sp => sp!.AnhSanPham)
                .Include(k => k.MaTKXuLyNavigation)
                .FirstOrDefaultAsync(k => k.MaKN == id);

            if (kn == null) return null;

            return new KhieuNaiAdminDetailViewModel
            {
                MaKN = kn.MaKN,
                MaTK = kn.MaTK,
                TenKhachHang = kn.MaTKNavigation.HoTen,
                EmailKhachHang = kn.MaTKNavigation.Email,
                SdtKhachHang = kn.MaTKNavigation.SoDienThoai,
                MaDH = kn.MaDH,
                TrangThaiDonHang = kn.MaDHNavigation?.TrangThai,
                NgayGiaoThanhCong = kn.MaDHNavigation?.NgayGiaoThanhCong,
                MaSP = kn.MaSP,
                TenSP = kn.MaSPNavigation?.TenSP,
                AnhSP = kn.MaSPNavigation?.AnhSanPham
                    .OrderByDescending(a => a.LaAnhChinh)
                    .Select(a => a.DuongDan)
                    .FirstOrDefault(),
                NoiDung = kn.NoiDung,
                NgayGui = kn.NgayGui,
                TrangThai = kn.TrangThai,
                PhanHoi = kn.PhanHoi,
                NgayPhanHoi = kn.NgayPhanHoi,
                MaTKXuLy = kn.MaTKXuLy,
                TenAdminXuLy = kn.MaTKXuLyNavigation?.HoTen
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> XuLyKhieuNaiAsync(
            int id, string trangThaiCu, string trangThaiMoi, string? phanHoi, int maTKAdmin)
        {
            if (id <= 0) return (false, _localizer["Common_Error"].Value);

            // Kiểm tra trạng thái cũ: nếu đã đóng thì không cho đổi
            if (trangThaiCu == TrangThaiKhieuNaiConstant.DaGiaiQuyet || trangThaiCu == TrangThaiKhieuNaiConstant.TuChoi)
            {
                return (false, _localizer["Admin_Complaint_Error_AlreadyClosed"].Value);
            }

            // Kiểm tra cặp chuyển đổi hợp lệ
            bool hopLe = (trangThaiCu == TrangThaiKhieuNaiConstant.ChoXuLy && trangThaiMoi == TrangThaiKhieuNaiConstant.DangXuLy) ||
                         (trangThaiCu == TrangThaiKhieuNaiConstant.ChoXuLy && trangThaiMoi == TrangThaiKhieuNaiConstant.DaGiaiQuyet) ||
                         (trangThaiCu == TrangThaiKhieuNaiConstant.ChoXuLy && trangThaiMoi == TrangThaiKhieuNaiConstant.TuChoi) ||
                         (trangThaiCu == TrangThaiKhieuNaiConstant.DangXuLy && trangThaiMoi == TrangThaiKhieuNaiConstant.DaGiaiQuyet) ||
                         (trangThaiCu == TrangThaiKhieuNaiConstant.DangXuLy && trangThaiMoi == TrangThaiKhieuNaiConstant.TuChoi);

            if (!hopLe)
            {
                return (false, _localizer["Admin_Complaint_Error_InvalidTransition"].Value);
            }

            // Validate phản hồi
            string? phanHoiClean = phanHoi?.Trim();

            if (trangThaiMoi == TrangThaiKhieuNaiConstant.DaGiaiQuyet || trangThaiMoi == TrangThaiKhieuNaiConstant.TuChoi)
            {
                if (string.IsNullOrWhiteSpace(phanHoiClean))
                {
                    return (false, _localizer["Admin_Complaint_Error_ResponseRequired"].Value);
                }
                if (phanHoiClean.Length < 10 || phanHoiClean.Length > 1000)
                {
                    return (false, _localizer["Admin_Complaint_Error_ResponseLength"].Value);
                }
            }
            else if (trangThaiMoi == TrangThaiKhieuNaiConstant.DangXuLy)
            {
                if (!string.IsNullOrWhiteSpace(phanHoiClean) && (phanHoiClean.Length < 10 || phanHoiClean.Length > 1000))
                {
                    return (false, _localizer["Admin_Complaint_Error_ResponseLength"].Value);
                }
            }

            bool coPhanHoiMoi = !string.IsNullOrWhiteSpace(phanHoiClean);
            var now = DateTime.Now;

            int affected;
            if (coPhanHoiMoi)
            {
                affected = await _context.KhieuNai
                    .Where(k => k.MaKN == id && k.TrangThai == trangThaiCu)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(k => k.TrangThai, trangThaiMoi)
                        .SetProperty(k => k.PhanHoi, phanHoiClean)
                        .SetProperty(k => k.NgayPhanHoi, now)
                        .SetProperty(k => k.MaTKXuLy, maTKAdmin));
            }
            else
            {
                affected = await _context.KhieuNai
                    .Where(k => k.MaKN == id && k.TrangThai == trangThaiCu)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(k => k.TrangThai, trangThaiMoi)
                        .SetProperty(k => k.MaTKXuLy, maTKAdmin));
            }

            if (affected == 0)
            {
                return (false, _localizer["Admin_Complaint_Error_Concurrency"].Value);
            }

            return (true, null);
        }

        private static string ChuanHoaTrangThai(string? trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai)) return BoLocTrangThaiConstant.DangMo;

            if (string.Equals(trangThai, BoLocTrangThaiConstant.DangMo, StringComparison.OrdinalIgnoreCase))
                return BoLocTrangThaiConstant.DangMo;

            if (string.Equals(trangThai, BoLocTrangThaiConstant.TatCa, StringComparison.OrdinalIgnoreCase))
                return BoLocTrangThaiConstant.TatCa;

            if (string.Equals(trangThai, TrangThaiKhieuNaiConstant.ChoXuLy, StringComparison.OrdinalIgnoreCase))
                return TrangThaiKhieuNaiConstant.ChoXuLy;

            if (string.Equals(trangThai, TrangThaiKhieuNaiConstant.DangXuLy, StringComparison.OrdinalIgnoreCase))
                return TrangThaiKhieuNaiConstant.DangXuLy;

            if (string.Equals(trangThai, TrangThaiKhieuNaiConstant.DaGiaiQuyet, StringComparison.OrdinalIgnoreCase))
                return TrangThaiKhieuNaiConstant.DaGiaiQuyet;

            if (string.Equals(trangThai, TrangThaiKhieuNaiConstant.TuChoi, StringComparison.OrdinalIgnoreCase))
                return TrangThaiKhieuNaiConstant.TuChoi;

            return BoLocTrangThaiConstant.DangMo;
        }
    }
}
