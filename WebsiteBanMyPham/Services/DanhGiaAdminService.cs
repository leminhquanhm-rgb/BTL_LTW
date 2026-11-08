using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class DanhGiaAdminService : IDanhGiaAdminService
    {
        private const int PageSizeFixed = 10;
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DanhGiaAdminService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<PaginatedList<DanhGiaAdminListItemViewModel>> LayDanhSachAsync(DanhGiaAdminFilterViewModel filter)
        {
            int page = filter.Trang < 1 ? 1 : filter.Trang;

            // Chuẩn hóa trạng thái: thiếu hoặc không hợp lệ -> ChoDuyet
            string trangThaiChuan = ChuanHoaTrangThai(filter.TrangThai);
            filter.TrangThai = trangThaiChuan;

            // Chuẩn hóa số sao: chỉ nhận 1-5
            byte? soSaoChuan = (filter.SoSao.HasValue && filter.SoSao.Value >= 1 && filter.SoSao.Value <= 5)
                ? filter.SoSao.Value
                : null;
            filter.SoSao = soSaoChuan;

            var query = _context.DanhGia.AsNoTracking();

            if (trangThaiChuan != BoLocTrangThaiConstant.TatCa)
            {
                query = query.Where(dg => dg.TrangThai == trangThaiChuan);
            }

            if (soSaoChuan.HasValue)
            {
                query = query.Where(dg => dg.SoSao == soSaoChuan.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(dg =>
                    dg.MaCTDHNavigation.MaSPNavigation.TenSP.Contains(kw) ||
                    dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.HoTen.Contains(kw) ||
                    dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.Email.Contains(kw));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(dg => dg.NgayTao)
                .Skip((page - 1) * PageSizeFixed)
                .Take(PageSizeFixed)
                .Select(dg => new DanhGiaAdminListItemViewModel
                {
                    MaDG = dg.MaDG,
                    MaCTDH = dg.MaCTDH,
                    MaSP = dg.MaCTDHNavigation.MaSP,
                    TenSP = dg.MaCTDHNavigation.MaSPNavigation.TenSP,
                    AnhSP = dg.MaCTDHNavigation.MaSPNavigation.AnhSanPham
                        .OrderByDescending(a => a.LaAnhChinh)
                        .Select(a => a.DuongDan)
                        .FirstOrDefault() ?? "/images/placeholder.svg",
                    MaDH = dg.MaCTDHNavigation.MaDH,
                    MaTK = dg.MaCTDHNavigation.MaDHNavigation.MaTK,
                    TenKhachHang = dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.HoTen,
                    EmailKhachHang = dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.Email,
                    SoSao = dg.SoSao,
                    NoiDung = dg.NoiDung,
                    TrangThai = dg.TrangThai,
                    NgayTao = dg.NgayTao
                })
                .ToListAsync();

            return new PaginatedList<DanhGiaAdminListItemViewModel>(items, totalItems, page, PageSizeFixed);
        }

        public async Task<DanhGiaAdminStatusCountViewModel> LaySoLuongTheoTrangThaiAsync(byte? soSao, string? keyword)
        {
            byte? soSaoChuan = (soSao.HasValue && soSao.Value >= 1 && soSao.Value <= 5) ? soSao.Value : null;

            var query = _context.DanhGia.AsNoTracking();

            if (soSaoChuan.HasValue)
            {
                query = query.Where(dg => dg.SoSao == soSaoChuan.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(dg =>
                    dg.MaCTDHNavigation.MaSPNavigation.TenSP.Contains(kw) ||
                    dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.HoTen.Contains(kw) ||
                    dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.Email.Contains(kw));
            }

            var counts = await query
                .GroupBy(dg => dg.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToListAsync();

            int choDuyet = counts.FirstOrDefault(c => c.TrangThai == TrangThaiDanhGiaConstant.ChoDuyet)?.Count ?? 0;
            int daDuyet = counts.FirstOrDefault(c => c.TrangThai == TrangThaiDanhGiaConstant.DaDuyet)?.Count ?? 0;
            int tuChoi = counts.FirstOrDefault(c => c.TrangThai == TrangThaiDanhGiaConstant.TuChoi)?.Count ?? 0;

            return new DanhGiaAdminStatusCountViewModel
            {
                ChoDuyet = choDuyet,
                DaDuyet = daDuyet,
                TuChoi = tuChoi,
                TatCa = choDuyet + daDuyet + tuChoi
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int id, string trangThaiCu, string trangThaiMoi)
        {
            if (id <= 0) return (false, _localizer["Common_Error"].Value);

            // Kiểm tra cặp chuyển đổi hợp lệ
            bool hopLe = (trangThaiCu == TrangThaiDanhGiaConstant.ChoDuyet && (trangThaiMoi == TrangThaiDanhGiaConstant.DaDuyet || trangThaiMoi == TrangThaiDanhGiaConstant.TuChoi)) ||
                         (trangThaiCu == TrangThaiDanhGiaConstant.DaDuyet && trangThaiMoi == TrangThaiDanhGiaConstant.TuChoi) ||
                         (trangThaiCu == TrangThaiDanhGiaConstant.TuChoi && trangThaiMoi == TrangThaiDanhGiaConstant.DaDuyet);

            if (!hopLe)
            {
                return (false, _localizer["Admin_Review_Error_InvalidTransition"].Value);
            }

            int affected = await _context.DanhGia
                .Where(dg => dg.MaDG == id && dg.TrangThai == trangThaiCu)
                .ExecuteUpdateAsync(s => s.SetProperty(dg => dg.TrangThai, trangThaiMoi));

            if (affected == 0)
            {
                return (false, _localizer["Admin_Review_Error_Concurrency"].Value);
            }

            return (true, null);
        }

        private static string ChuanHoaTrangThai(string? trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai)) return TrangThaiDanhGiaConstant.ChoDuyet;

            if (string.Equals(trangThai, BoLocTrangThaiConstant.TatCa, StringComparison.OrdinalIgnoreCase))
                return BoLocTrangThaiConstant.TatCa;

            if (string.Equals(trangThai, TrangThaiDanhGiaConstant.ChoDuyet, StringComparison.OrdinalIgnoreCase))
                return TrangThaiDanhGiaConstant.ChoDuyet;

            if (string.Equals(trangThai, TrangThaiDanhGiaConstant.DaDuyet, StringComparison.OrdinalIgnoreCase))
                return TrangThaiDanhGiaConstant.DaDuyet;

            if (string.Equals(trangThai, TrangThaiDanhGiaConstant.TuChoi, StringComparison.OrdinalIgnoreCase))
                return TrangThaiDanhGiaConstant.TuChoi;

            return TrangThaiDanhGiaConstant.ChoDuyet;
        }
    }
}
