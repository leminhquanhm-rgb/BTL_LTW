using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly ILogger<GioHangService> _logger;

        public GioHangService(WebsiteBanMyPhamContext context, ILogger<GioHangService> logger)
        {
            _context = context;
            _logger = logger;
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
                    item.ThongBaoTrangThai = "Sản phẩm đã ngừng kinh doanh";
                }
                else if (item.SoLuongTon <= 0)
                {
                    item.ThongBaoTrangThai = "Sản phẩm đã hết hàng";
                }
                else if (item.SoLuong > item.SoLuongTon)
                {
                    item.ThongBaoTrangThai = $"Số lượng trong giỏ vượt quá tồn kho (còn lại: {item.SoLuongTon})";
                }

                result.Items.Add(item);
            }

            return result;
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemVaoGioAsync(int maTK, int maSP, int soLuong)
        {
            if (soLuong <= 0)
            {
                return (false, "Số lượng thêm vào giỏ phải lớn hơn 0.");
            }

            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return (false, "Chỉ tài khoản khách hàng đang hoạt động mới được sử dụng giỏ hàng.");
            }

            var sp = await _context.SanPham.AsNoTracking().FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null || !sp.TrangThai)
            {
                return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            if (sp.SoLuongTon <= 0)
            {
                return (false, "Sản phẩm hiện đã hết hàng.");
            }

            var dongGioHienTai = await _context.GioHang.FirstOrDefaultAsync(g => g.MaTK == maTK && g.MaSP == maSP);

            if (dongGioHienTai != null)
            {
                int soLuongSauKhiCong = dongGioHienTai.SoLuong + soLuong;
                if (soLuongSauKhiCong > sp.SoLuongTon)
                {
                    return (false, $"Số lượng trong giỏ ({dongGioHienTai.SoLuong}) + thêm ({soLuong}) vượt quá tồn kho hiện tại ({sp.SoLuongTon}).");
                }

                dongGioHienTai.SoLuong = soLuongSauKhiCong;
                dongGioHienTai.NgayThem = DateTime.Now;
                await _context.SaveChangesAsync();
                return (true, null);
            }

            if (soLuong > sp.SoLuongTon)
            {
                return (false, $"Số lượng yêu cầu ({soLuong}) vượt quá số lượng tồn kho ({sp.SoLuongTon}).");
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
                        return (false, $"Số lượng sau khi cộng dồn ({tongMoi}) vượt quá tồn kho ({sp.SoLuongTon}).");
                    }
                    dongDaCo.SoLuong = tongMoi;
                    dongDaCo.NgayThem = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return (true, null);
                }

                return (false, "Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại.");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatSoLuongAsync(int maTK, int maGH, int soLuongMoi)
        {
            if (soLuongMoi <= 0)
            {
                return (false, "Số lượng phải lớn hơn 0.");
            }

            var hopLe = await KiemTraKhachHangHopLeAsync(maTK);
            if (!hopLe)
            {
                return (false, "Thao tác không hợp lệ.");
            }

            var dongGio = await _context.GioHang
                .Include(g => g.MaSPNavigation)
                .FirstOrDefaultAsync(g => g.MaGH == maGH && g.MaTK == maTK);

            if (dongGio == null)
            {
                return (false, "Không tìm thấy sản phẩm trong giỏ hàng hoặc bạn không có quyền thao tác.");
            }

            var sp = dongGio.MaSPNavigation;
            if (!sp.TrangThai)
            {
                return (false, "Sản phẩm này đã ngừng kinh doanh.");
            }

            if (soLuongMoi > sp.SoLuongTon)
            {
                return (false, $"Số lượng yêu cầu ({soLuongMoi}) vượt quá tồn kho hiện tại ({sp.SoLuongTon}).");
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
                return (false, "Thao tác không hợp lệ.");
            }

            var dongGio = await _context.GioHang.FirstOrDefaultAsync(g => g.MaGH == maGH && g.MaTK == maTK);
            if (dongGio == null)
            {
                return (false, "Không tìm thấy dòng giỏ hàng hoặc bạn không có quyền xóa.");
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

