using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class KhuyenMaiService : IKhuyenMaiService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILocalizationHelper _localizationHelper;

        public KhuyenMaiService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer,
            ILocalizationHelper localizationHelper)
        {
            _context = context;
            _localizer = localizer;
            _localizationHelper = localizationHelper;
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
                result.ThongBao = _localizer["Msg_Promo_Required"].Value;
                return result;
            }

            var codeChuanHoa = code.Trim();

            var km = await _context.KhuyenMai
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Code == codeChuanHoa);

            if (km == null)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_NotFound"].Value;
                return result;
            }

            if (!km.TrangThai)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_Inactive"].Value;
                return result;
            }

            var now = DateTime.Now;
            if (now < km.NgayBatDau)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_NotStarted", _localizationHelper.FormatDateTime(km.NgayBatDau)].Value;
                return result;
            }

            if (now > km.NgayKetThuc)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_Expired"].Value;
                return result;
            }

            if (km.SoLuotToiDa.HasValue && km.SoLuotDaDung >= km.SoLuotToiDa.Value)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_OutOfUsage"].Value;
                return result;
            }

            if (tongTienHang < km.DonToiThieu)
            {
                result.HopLe = false;
                result.ThongBao = _localizer["Msg_Promo_MinOrderAmountNotMet", _localizationHelper.FormatCurrency(km.DonToiThieu)].Value;
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
            result.ThongBao = _localizer["Msg_Promo_ApplySuccess", _localizationHelper.FormatCurrency(tienGiam)].Value;
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
