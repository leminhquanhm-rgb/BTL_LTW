using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class KhieuNaiService : IKhieuNaiService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILocalizationHelper _localizationHelper;
        private readonly ILogger<KhieuNaiService> _logger;

        public KhieuNaiService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer,
            ILocalizationHelper localizationHelper,
            ILogger<KhieuNaiService> logger)
        {
            _context = context;
            _localizer = localizer;
            _localizationHelper = localizationHelper;
            _logger = logger;
        }

        public async Task<(bool Success, string? ErrorMessage, LoaiLoiKhieuNai LoaiLoi, int? MaKN)> TaoKhieuNaiAsync(
            int maTK, int maDH, int? maSP, string noiDung)
        {
            if (maDH <= 0)
            {
                return (false, _localizer["Complaint_Error_NoOrderSelected"].Value, LoaiLoiKhieuNai.DuLieuKhongHopLe, null);
            }

            string cleanContent = noiDung?.Trim() ?? string.Empty;
            if (cleanContent.Length < KhieuNaiConstant.DoDaiNoiDungToiThieu || cleanContent.Length > KhieuNaiConstant.DoDaiNoiDungToiDa)
            {
                return (false, _localizer["Complaint_Error_ContentLength"].Value, LoaiLoiKhieuNai.DuLieuKhongHopLe, null);
            }

            // 1. Kiểm tra giới hạn 10 khiếu nại đang mở (ChoXuLy hoặc DangXuLy)
            int countOpen = await _context.KhieuNai
                .AsNoTracking()
                .CountAsync(k => k.MaTK == maTK && (k.TrangThai == TrangThaiKhieuNaiConstant.ChoXuLy || k.TrangThai == TrangThaiKhieuNaiConstant.DangXuLy));

            if (countOpen >= KhieuNaiConstant.SoLuongKhieuNaiMoToiDa)
            {
                return (false, _localizer["Complaint_Error_MaxOpenLimit"].Value, LoaiLoiKhieuNai.GioiHanSoLuong, null);
            }

            // 2. Kiểm tra đơn hàng thuộc khách
            var donHang = await _context.DonHang
                .Include(d => d.ChiTietDonHang)
                .FirstOrDefaultAsync(d => d.MaDH == maDH && d.MaTK == maTK);

            if (donHang == null)
            {
                return (false, _localizer["Complaint_Error_OrderNotOwned"].Value, LoaiLoiKhieuNai.KhongTimThay, null);
            }

            // 3. Đơn hàng phải ở trạng thái HoanThanh
            if (donHang.TrangThai != TrangThaiDonHangConstant.HoanThanh)
            {
                return (false, _localizer["Complaint_Error_OrderNotCompleted"].Value, LoaiLoiKhieuNai.XungDotTrangThai, null);
            }

            // 4. Kiểm tra thời hạn 10 ngày kể từ ngày giao hàng thành công
            if (!donHang.NgayGiaoThanhCong.HasValue ||
                DateTime.Now > donHang.NgayGiaoThanhCong.Value.AddDays(KhieuNaiConstant.SoNgayToiDaSauGiaoHang))
            {
                return (false, _localizer["Complaint_Error_OrderPastDeadline"].Value, LoaiLoiKhieuNai.QuaHanKhieuNai, null);
            }

            // 5. Nếu có MaSP: sản phẩm phải thuộc đơn hàng
            if (maSP.HasValue && maSP.Value > 0)
            {
                bool spInOrder = donHang.ChiTietDonHang.Any(c => c.MaSP == maSP.Value);
                if (!spInOrder)
                {
                    return (false, _localizer["Complaint_Error_ProductNotInOrder"].Value, LoaiLoiKhieuNai.DuLieuKhongHopLe, null);
                }
            }

            var kn = new KhieuNai
            {
                MaTK = maTK,
                MaDH = maDH,
                MaSP = (maSP.HasValue && maSP.Value > 0) ? maSP.Value : null,
                NoiDung = cleanContent,
                NgayGui = DateTime.Now,
                TrangThai = TrangThaiKhieuNaiConstant.ChoXuLy,
                PhanHoi = null,
                NgayPhanHoi = null,
                MaTKXuLy = null
            };

            _context.KhieuNai.Add(kn);

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, LoaiLoiKhieuNai.KhongCoLoi, kn.MaKN);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi tạo khiếu nại cho MaDH {MaDH} bởi MaTK {MaTK}", maDH, maTK);
                return (false, _localizer["Common_Error"].Value, LoaiLoiKhieuNai.DuLieuKhongHopLe, null);
            }
        }

        public async Task<KhieuNaiListViewModel> LayDanhSachAsync(int maTK, int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.KhieuNai
                .AsNoTracking()
                .Where(k => k.MaTK == maTK);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderByDescending(k => k.NgayGui)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(k => new KhieuNaiItemViewModel
                {
                    MaKN = k.MaKN,
                    MaDH = k.MaDH,
                    MaSP = k.MaSP,
                    TenSP = k.MaSPNavigation != null ? k.MaSPNavigation.TenSP : null,
                    NoiDung = k.NoiDung,
                    NgayGui = k.NgayGui,
                    TrangThai = k.TrangThai,
                    PhanHoi = k.PhanHoi,
                    NgayPhanHoi = k.NgayPhanHoi
                })
                .ToListAsync();

            return new KhieuNaiListViewModel
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<KhieuNaiChiTietViewModel?> LayChiTietAsync(int maTK, int maKN)
        {
            var k = await _context.KhieuNai
                .AsNoTracking()
                .Where(x => x.MaKN == maKN && x.MaTK == maTK)
                .Include(x => x.MaSPNavigation)
                    .ThenInclude(sp => sp!.AnhSanPham)
                .Include(x => x.MaDHNavigation)
                    .ThenInclude(dh => dh!.ChiTietDonHang)
                .FirstOrDefaultAsync();

            if (k == null) return null;

            string? anhSP = null;
            decimal? donGiaSP = null;

            if (k.MaSPNavigation != null)
            {
                anhSP = k.MaSPNavigation.AnhSanPham
                    .OrderByDescending(a => a.LaAnhChinh)
                    .ThenBy(a => a.MaAnh)
                    .FirstOrDefault()?.DuongDan;

                // Lấy đơn giá lúc mua từ ChiTietDonHang nếu có
                if (k.MaDHNavigation != null)
                {
                    var ct = k.MaDHNavigation.ChiTietDonHang.FirstOrDefault(c => c.MaSP == k.MaSP);
                    if (ct != null)
                    {
                        donGiaSP = ct.DonGia;
                    }
                }

                if (!donGiaSP.HasValue)
                {
                    donGiaSP = k.MaSPNavigation.Gia;
                }
            }

            return new KhieuNaiChiTietViewModel
            {
                MaKN = k.MaKN,
                MaDH = k.MaDH,
                MaSP = k.MaSP,
                TenSP = k.MaSPNavigation?.TenSP,
                DuongDanAnhSP = anhSP,
                DonGiaSP = donGiaSP,
                NoiDung = k.NoiDung,
                NgayGui = k.NgayGui,
                TrangThai = k.TrangThai,
                PhanHoi = k.PhanHoi,
                NgayPhanHoi = k.NgayPhanHoi
            };
        }

        public async Task<List<DonHangSanPhamOptionViewModel>> LayDonHangVaSanPhamCuaKhachAsync(int maTK)
        {
            DateTime hanChot = DateTime.Now.AddDays(-KhieuNaiConstant.SoNgayToiDaSauGiaoHang);

            var orders = await _context.DonHang
                .AsNoTracking()
                .Where(d => d.MaTK == maTK
                            && d.TrangThai == TrangThaiDonHangConstant.HoanThanh
                            && d.NgayGiaoThanhCong != null
                            && d.NgayGiaoThanhCong >= hanChot)
                .Include(d => d.ChiTietDonHang)
                .OrderByDescending(d => d.NgayGiaoThanhCong)
                .ToListAsync();

            var result = new List<DonHangSanPhamOptionViewModel>();
            foreach (var d in orders)
            {
                result.Add(new DonHangSanPhamOptionViewModel
                {
                    MaDH = d.MaDH,
                    MaDonHangText = _localizationHelper.MaDonHang(d.MaDH),
                    NgayGiaoThanhCong = d.NgayGiaoThanhCong!.Value,
                    SanPhams = d.ChiTietDonHang.Select(c => new SanPhamOptionItem
                    {
                        MaSP = c.MaSP,
                        TenSP = c.TenSP
                    }).ToList()
                });
            }

            return result;
        }
    }
}

