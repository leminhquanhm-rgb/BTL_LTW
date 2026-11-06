using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly ILogger<GioHangService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GioHangService(
            WebsiteBanMyPhamContext context,
            ILogger<GioHangService> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
        }

        private async Task<bool> KiemTraKhachHangHopLeAsync(int maTK)
        {
            var user = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTK == maTK);
            return user != null && user.TrangThai && user.VaiTro == VaiTroConstant.KhachHang;
        }

        public async Task<GioHangViewModel> LayGioHangAsync(int maTK)
        {
            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return new GioHangViewModel();
            }

            var query = await _context.GioHang
                .AsNoTracking()
                .Where(g => g.MaTK == maTK)
                .Include(g => g.MaSPNavigation)
                    .ThenInclude(sp => sp.AnhSanPham)
                .OrderByDescending(g => g.NgayThem)
                .ToListAsync();

            var result = new GioHangViewModel();

            foreach (var g in query)
            {
                var sp = g.MaSPNavigation;
                var anhChinh = sp.AnhSanPham.FirstOrDefault(a => a.LaAnhChinh)?.DuongDan 
                               ?? sp.AnhSanPham.FirstOrDefault()?.DuongDan 
                               ?? "/images/no-image.png";

                var item = new GioHangItemViewModel
                {
                    MaGH = g.MaGH,
                    MaSP = g.MaSP,
                    TenSP = sp.TenSP,
                    DuongDanAnh = anhChinh,
                    GiaHienTai = sp.Gia,
                    SoLuong = g.SoLuong,
                    SoLuongTon = sp.SoLuongTon,
                    TrangThaiKinhDoanh = sp.TrangThai
                };

                if (!item.TrangThaiKinhDoanh)
                {
                    item.ThongBaoTrangThai = _localizer["Msg_Cart_StatusStopped"].Value;
                }
                else if (item.SoLuongTon <= 0)
                {
                    item.ThongBaoTrangThai = _localizer["Msg_Cart_StatusOutOfStock"].Value;
                }
                else if (item.SoLuong > item.SoLuongTon)
                {
                    item.ThongBaoTrangThai = _localizer["Msg_Cart_StatusExceedStock", item.SoLuongTon].Value;
                }

                result.Items.Add(item);
            }

            return result;
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemVaoGioAsync(int maTK, int maSP, int soLuong)
        {
            if (soLuong <= 0)
            {
                return (false, _localizer["Msg_Cart_QuantityPositive"].Value);
            }

            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return (false, _localizer["Msg_Cart_OnlyCustomer"].Value);
            }

            var sp = await _context.SanPham.AsNoTracking().FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null || !sp.TrangThai)
            {
                return (false, _localizer["Msg_Cart_ProductStopped"].Value);
            }

            if (sp.SoLuongTon <= 0)
            {
                return (false, _localizer["Msg_Cart_ProductOutOfStock"].Value);
            }

            var dongGioHienTai = await _context.GioHang.FirstOrDefaultAsync(g => g.MaTK == maTK && g.MaSP == maSP);

            if (dongGioHienTai != null)
            {
                int soLuongSauKhiCong = dongGioHienTai.SoLuong + soLuong;
                if (soLuongSauKhiCong > sp.SoLuongTon)
                {
                    return (false, _localizer["Msg_Cart_ExceedStockDetailed", dongGioHienTai.SoLuong, soLuong, sp.SoLuongTon].Value);
                }

                dongGioHienTai.SoLuong = soLuongSauKhiCong;
                dongGioHienTai.NgayThem = DateTime.Now;
                await _context.SaveChangesAsync();
                return (true, null);
            }

            if (soLuong > sp.SoLuongTon)
            {
                return (false, _localizer["Msg_Cart_ExceedStock", soLuong, sp.SoLuongTon].Value);
            }

            var dongMoi = new GioHang
            {
                MaTK = maTK,
                MaSP = maSP,
                SoLuong = soLuong,
                NgayThem = DateTime.Now
            };

            try
            {
                _context.GioHang.Add(dongMoi);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                // Bắt xung đột race condition UNIQUE(MaTK, MaSP) khi hai request đồng thời
                _logger.LogWarning(ex, "Xảy ra xung đột UNIQUE giỏ hàng cho MaTK {MaTK}, MaSP {MaSP}. Xử lý cộng dồn.", maTK, maSP);
                _context.ChangeTracker.Clear();

                var dongDaCo = await _context.GioHang.FirstOrDefaultAsync(g => g.MaTK == maTK && g.MaSP == maSP);
                if (dongDaCo != null)
                {
                    int tongMoi = dongDaCo.SoLuong + soLuong;
                    if (tongMoi > sp.SoLuongTon)
                    {
                        return (false, _localizer["Msg_Cart_ExceedStockAccumulated", tongMoi, sp.SoLuongTon].Value);
                    }
                    dongDaCo.SoLuong = tongMoi;
                    dongDaCo.NgayThem = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return (true, null);
                }

                return (false, _localizer["Msg_Cart_AddFailed"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatSoLuongAsync(int maTK, int maGH, int soLuongMoi)
        {
            if (soLuongMoi <= 0)
            {
                return (false, _localizer["Msg_Cart_QuantityMustBePositive"].Value);
            }

            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return (false, _localizer["Msg_Cart_InvalidAction"].Value);
            }

            var dongGio = await _context.GioHang
                .Include(g => g.MaSPNavigation)
                .FirstOrDefaultAsync(g => g.MaGH == maGH && g.MaTK == maTK);

            if (dongGio == null)
            {
                return (false, _localizer["Msg_Cart_ItemNotFoundOrNotOwned"].Value);
            }

            var sp = dongGio.MaSPNavigation;
            if (!sp.TrangThai)
            {
                return (false, _localizer["Msg_Cart_ItemProductStopped"].Value);
            }

            if (soLuongMoi > sp.SoLuongTon)
            {
                return (false, _localizer["Msg_Cart_ExceedStock", soLuongMoi, sp.SoLuongTon].Value);
            }

            dongGio.SoLuong = soLuongMoi;
            dongGio.NgayThem = DateTime.Now;
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaKhoiGioAsync(int maTK, int maGH)
        {
            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return (false, _localizer["Msg_Cart_InvalidAction"].Value);
            }

            var dongGio = await _context.GioHang.FirstOrDefaultAsync(g => g.MaGH == maGH && g.MaTK == maTK);
            if (dongGio == null)
            {
                return (false, _localizer["Msg_Cart_ItemDeleteNotFound"].Value);
            }

            _context.GioHang.Remove(dongGio);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<int> DemSoLuongAsync(int maTK)
        {
            return await _context.GioHang
                .AsNoTracking()
                .Where(g => g.MaTK == maTK)
                .SumAsync(g => g.SoLuong);
        }
    }
}
