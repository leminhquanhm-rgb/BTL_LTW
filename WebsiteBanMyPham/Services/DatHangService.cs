using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class DatHangService : IDatHangService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IKhuyenMaiService _khuyenMaiService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DatHangService(
            WebsiteBanMyPhamContext context,
            IKhuyenMaiService khuyenMaiService,
            IConfiguration configuration,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _khuyenMaiService = khuyenMaiService;
            _configuration = configuration;
            _localizer = localizer;
        }

        public async Task<(bool HopLe, string? ThongBaoLoi, DatHangViewModel? ViewModel)> ChuanBiDatHangAsync(
            int maTK, List<int>? maGH, int? maSP, int? soLuong, string? codeKM)
        {
            bool hasCart = maGH != null && maGH.Any();
            bool hasBuyNow = maSP.HasValue && maSP > 0 && soLuong.HasValue && soLuong >= 1 && soLuong <= 99;

            if ((hasCart && hasBuyNow) || (!hasCart && !hasBuyNow))
            {
                return (false, _localizer["Msg_Order_SourceInvalid"].Value, null);
            }

            var items = new List<DatHangItemViewModel>();
            string nguonDat;
            var danhSachMaGH = new List<int>();
            int? maSPMuaNgay = null;
            int soLuongMuaNgay = 1;

            if (hasCart)
            {
                nguonDat = NguonDatHangConstant.TuGio;
                danhSachMaGH = maGH!.Distinct().Take(100).ToList();

                var cartLines = await _context.GioHang
                    .AsNoTracking()
                    .Where(g => g.MaTK == maTK && danhSachMaGH.Contains(g.MaGH))
                    .Include(g => g.MaSPNavigation)
                    .ThenInclude(s => s.AnhSanPham)
                    .ToListAsync();

                if (cartLines.Count != danhSachMaGH.Count)
                {
                    return (false, _localizer["Msg_Order_CartItemsNotOwned"].Value, null);
                }

                foreach (var line in cartLines)
                {
                    var sp = line.MaSPNavigation;
                    bool hopLe = sp.TrangThai && sp.SoLuongTon >= line.SoLuong && line.SoLuong > 0 && sp.SoLuongTon > 0;
                    string? thongBao = null;
                    if (!sp.TrangThai)
                    {
                        thongBao = _localizer["Cart_ItemStoppedWarning"].Value;
                    }
                    else if (sp.SoLuongTon <= 0)
                    {
                        thongBao = _localizer["Cart_ItemOutOfStockWarning"].Value;
                    }
                    else if (line.SoLuong > sp.SoLuongTon)
                    {
                        thongBao = string.Format(_localizer["Cart_ItemExceedWarning"].Value, sp.SoLuongTon);
                    }

                    var anh = sp.AnhSanPham
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.MaAnh)
                        .FirstOrDefault()?.DuongDan;

                    items.Add(new DatHangItemViewModel
                    {
                        MaSP = sp.MaSP,
                        MaGH = line.MaGH,
                        TenSP = sp.TenSP,
                        DuongDanAnh = anh,
                        GiaHienTai = sp.Gia,
                        GiaGoc = sp.GiaGoc,
                        SoLuong = line.SoLuong,
                        SoLuongTon = sp.SoLuongTon,
                        TrangThaiSP = sp.TrangThai,
                        HopLeDeDat = hopLe,
                        ThongBaoTrangThai = thongBao
                    });
                }
            }
            else
            {
                nguonDat = NguonDatHangConstant.MuaNgay;
                maSPMuaNgay = maSP!.Value;
                soLuongMuaNgay = soLuong!.Value;

                var sp = await _context.SanPham
                    .AsNoTracking()
                    .Include(s => s.AnhSanPham)
                    .FirstOrDefaultAsync(s => s.MaSP == maSPMuaNgay.Value);

                if (sp == null)
                {
                    return (false, _localizer["Msg_Order_ProductNotFound"].Value, null);
                }

                bool hopLe = sp.TrangThai && sp.SoLuongTon >= soLuongMuaNgay && soLuongMuaNgay > 0 && sp.SoLuongTon > 0;
                string? thongBao = null;
                if (!sp.TrangThai)
                {
                    thongBao = _localizer["Cart_ItemStoppedWarning"].Value;
                }
                else if (sp.SoLuongTon <= 0)
                {
                    thongBao = _localizer["Cart_ItemOutOfStockWarning"].Value;
                }
                else if (soLuongMuaNgay > sp.SoLuongTon)
                {
                    thongBao = string.Format(_localizer["Cart_ItemExceedWarning"].Value, sp.SoLuongTon);
                }

                var anh = sp.AnhSanPham
                    .OrderByDescending(a => a.LaAnhChinh)
                    .ThenBy(a => a.MaAnh)
                    .FirstOrDefault()?.DuongDan;

                items.Add(new DatHangItemViewModel
                {
                    MaSP = sp.MaSP,
                    MaGH = null,
                    TenSP = sp.TenSP,
                    DuongDanAnh = anh,
                    GiaHienTai = sp.Gia,
                    GiaGoc = sp.GiaGoc,
                    SoLuong = soLuongMuaNgay,
                    SoLuongTon = sp.SoLuongTon,
                    TrangThaiSP = sp.TrangThai,
                    HopLeDeDat = hopLe,
                    ThongBaoTrangThai = thongBao
                });
            }

            decimal tongTienHang = items.Sum(i => i.ThanhTien);
            decimal tienGiam = 0;
            decimal tongThanhToan = tongTienHang;
            string? appliedCode = null;

            if (!string.IsNullOrWhiteSpace(codeKM))
            {
                var kmResult = await _khuyenMaiService.TinhGiamAsync(codeKM.Trim(), tongTienHang);
                if (kmResult.HopLe)
                {
                    appliedCode = kmResult.Code;
                    tienGiam = kmResult.TienGiam;
                    tongThanhToan = kmResult.TongThanhToan;
                }
            }

            // Kiểm tra cấu hình ChoPhepMoPhong
            bool choPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");

            // Lấy danh sách phương thức thanh toán đang hoạt động
            var queryPt = _context.PhuongThucThanhToan
                .AsNoTracking()
                .Where(p => p.TrangThai);

            // Điều chỉnh 3: Nếu cờ mô phỏng tắt, chỉ hiển thị COD
            if (!choPhepMoPhong)
            {
                queryPt = queryPt.Where(p => p.TenPT == PhuongThucThanhToanConstant.COD);
            }

            var ptEntities = await queryPt.ToListAsync();
            var danhSachPt = ptEntities.Select(p => new PhuongThucThanhToanViewModel
            {
                MaPT = p.MaPT,
                TenPT = p.TenPT,
                ResourceKey = $"Payment_Method_{p.TenPT}",
                MoTa = p.MoTa,
                IsOnline = p.TenPT == PhuongThucThanhToanConstant.Online,
                IsSelected = p.TenPT == PhuongThucThanhToanConstant.COD
            }).ToList();

            // Nếu COD có trong danh sách, chọn COD; nếu không, chọn phương thức đầu tiên
            var ptChon = danhSachPt.FirstOrDefault(p => p.TenPT == PhuongThucThanhToanConstant.COD)
                         ?? danhSachPt.FirstOrDefault();
            if (ptChon != null)
            {
                ptChon.IsSelected = true;
            }

            // Điền sẵn thông tin người nhận từ tài khoản
            var user = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTK == maTK);

            var vm = new DatHangViewModel
            {
                NguonDat = nguonDat,
                DanhSachMaGH = danhSachMaGH,
                MaSPMuaNgay = maSPMuaNgay,
                SoLuongMuaNgay = soLuongMuaNgay,
                Items = items,
                TongTienHang = tongTienHang,
                CodeKM = appliedCode,
                TienGiam = tienGiam,
                TongThanhToan = tongThanhToan,
                DanhSachPhuongThuc = danhSachPt,
                ChoPhepMoPhong = choPhepMoPhong,
                TenNguoiNhan = user?.HoTen ?? string.Empty,
                SdtNguoiNhan = user?.SoDienThoai ?? string.Empty,
                DiaChiGiao = user?.DiaChi ?? string.Empty,
                MaPTChon = ptChon?.MaPT ?? 0,
                MaChongDatTrung = string.Empty
            };

            return (true, null, vm);
        }

        public async Task<(bool HopLe, int StatusCode, string? ErrorTitle, string? ErrorDetail, decimal TongTienHang)> KiemTraNguonMuaNgayChoKhuyenMaiAsync(
            int maSP, int soLuong)
        {
            if (soLuong < 1 || soLuong > 99)
            {
                return (false, StatusCodes.Status400BadRequest,
                    _localizer["Api_Error_InvalidPromoRequest"].Value,
                    _localizer["Validation_RangeMin"].Value, 0);
            }

            var sp = await _context.SanPham.AsNoTracking().FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null)
            {
                return (false, StatusCodes.Status404NotFound,
                    _localizer["Api_Error_NotFoundTitle"].Value,
                    _localizer["Msg_Order_ProductNotFound"].Value, 0);
            }

            if (!sp.TrangThai || sp.SoLuongTon < soLuong)
            {
                return (false, StatusCodes.Status400BadRequest,
                    _localizer["Api_Error_InvalidPromoRequest"].Value,
                    _localizer["Msg_Order_OutOfStock"].Value, 0);
            }

            decimal tongTienHang = sp.Gia * soLuong;
            return (true, StatusCodes.Status200OK, null, null, tongTienHang);
        }

        public async Task<(bool HopLe, string? ErrorMessage, string? TenPT)> KiemTraPhuongThucThanhToanAsync(int maPT)
        {
            var pt = await _context.PhuongThucThanhToan.AsNoTracking().FirstOrDefaultAsync(p => p.MaPT == maPT);
            if (pt == null || !pt.TrangThai)
            {
                return (false, _localizer["Msg_Order_PaymentMethodUnavailable"].Value, null);
            }

            if (pt.TenPT == PhuongThucThanhToanConstant.Online)
            {
                bool choPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");
                if (!choPhepMoPhong)
                {
                    return (false, _localizer["PaymentSim_NotAllowed"].Value, pt.TenPT);
                }
            }

            return (true, null, pt.TenPT);
        }
    }
}
