using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class KhuyenMaiService : IKhuyenMaiService
    {
        private readonly WebsiteBanMyPhamContext _context;

        public KhuyenMaiService(WebsiteBanMyPhamContext context)
        {
            _context = context;
        }

        public async Task<KhuyenMaiKetQuaViewModel> TinhGiamAsync(string? code, decimal tongTienHang)
        {
            var result = new KhuyenMaiKetQuaViewModel
            {
                TongTienHang = tongTienHang
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                result.HopLe = false;
                result.ThongBao = "Mã khuyến mãi không được để trống.";
                return result;
            }

            var codeChuanHoa = code.Trim();

            var km = await _context.KhuyenMai
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Code == codeChuanHoa);

            if (km == null)
            {
                result.HopLe = false;
                result.ThongBao = "Mã khuyến mãi không tồn tại trên hệ thống.";
                return result;
            }

            if (!km.TrangThai)
            {
                result.HopLe = false;
                result.ThongBao = "Chương trình khuyến mãi này đã tạm ngưng hoạt động.";
                return result;
            }

            var now = DateTime.Now;
            if (now < km.NgayBatDau)
            {
                result.HopLe = false;
                result.ThongBao = $"Chương trình khuyến mãi chưa bắt đầu (bắt đầu từ {km.NgayBatDau:dd/MM/yyyy}).";
                return result;
            }

            if (now > km.NgayKetThuc)
            {
                result.HopLe = false;
                result.ThongBao = "Mã khuyến mãi này đã hết hạn sử dụng.";
                return result;
            }

            if (km.SoLuotToiDa.HasValue && km.SoLuotDaDung >= km.SoLuotToiDa.Value)
            {
                result.HopLe = false;
                result.ThongBao = "Mã khuyến mãi này đã hết lượt sử dụng.";
                return result;
            }

            if (tongTienHang < km.DonToiThieu)
            {
                result.HopLe = false;
                result.ThongBao = $"Đơn hàng chưa đạt giá trị tối thiểu để áp dụng mã (yêu cầu từ {km.DonToiThieu:N0}đ).";
                return result;
            }

            decimal tienGiam = 0;
            if (km.LoaiGiam == LoaiGiamGiaConstant.PhanTram)
            {
                // Làm tròn xuống số nguyên đến đồng theo Điều chỉnh 6
                tienGiam = Math.Floor(tongTienHang * km.GiaTriGiam / 100m);
                if (km.GiamToiDa.HasValue && km.GiamToiDa.Value > 0)
                {
                    tienGiam = Math.Min(tienGiam, km.GiamToiDa.Value);
                }
            }
            else
            {
                tienGiam = km.GiaTriGiam;
            }

            // Tiền giảm không bao giờ được vượt quá tổng tiền hàng
            tienGiam = Math.Min(tienGiam, tongTienHang);

            result.HopLe = true;
            result.ThongBao = $"Áp dụng mã thành công! Bạn được giảm {tienGiam:N0}đ.";
            result.MaKM = km.MaKM;
            result.Code = km.Code;
            result.TenKM = km.TenKM;
            result.LoaiGiam = km.LoaiGiam;
            result.GiaTriGiam = km.GiaTriGiam;
            result.TienGiam = tienGiam;

            return result;
        }

        public async Task<List<KhuyenMaiItemViewModel>> LayDanhSachMaKhaDungAsync(decimal tongTienHang)
        {
            var now = DateTime.Now;

            var query = await _context.KhuyenMai
                .AsNoTracking()
                .Where(k => k.TrangThai 
                            && now >= k.NgayBatDau 
                            && now <= k.NgayKetThuc 
                            && (k.SoLuotToiDa == null || k.SoLuotDaDung < k.SoLuotToiDa)
                            && tongTienHang >= k.DonToiThieu)
                .OrderByDescending(k => k.GiaTriGiam)
                .ToListAsync();

            return query.Select(k => new KhuyenMaiItemViewModel
            {
                MaKM = k.MaKM,
                Code = k.Code,
                TenKM = k.TenKM,
                LoaiGiam = k.LoaiGiam,
                GiaTriGiam = k.GiaTriGiam,
                GiamToiDa = k.GiamToiDa,
                DonToiThieu = k.DonToiThieu,
                NgayKetThuc = k.NgayKetThuc,
                SoLuotToiDa = k.SoLuotToiDa,
                SoLuotDaDung = k.SoLuotDaDung
            }).ToList();
        }
    }
}

