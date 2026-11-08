using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class ExcelMaxRowsExceededException : Exception
    {
        public ExcelMaxRowsExceededException() : base("Dữ liệu xuất vượt quá giới hạn 50.000 dòng.") { }
    }

    public class ThongKeService : IThongKeService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly ILocalizationHelper _locHelper;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ThongKeService(
            WebsiteBanMyPhamContext context,
            ILocalizationHelper locHelper,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _locHelper = locHelper;
            _localizer = localizer;
        }

        public async Task<DashboardViewModel> LayDuLieuDashboardAsync(int? nam = null)
        {
            var now = DateTime.Now;
            var queryHoanThanh = _context.DonHang
                .AsNoTracking()
                .Where(d => d.TrangThai == TrangThaiDonHangConstant.HoanThanh);

            // 1. Chỉ số KPI
            decimal tongDoanhThu = await queryHoanThanh
                .SumAsync(d => (decimal?)d.TongThanhToan) ?? 0m;

            decimal doanhThuThangHienTai = await queryHoanThanh
                .Where(d => d.NgayGiaoThanhCong != null &&
                            d.NgayGiaoThanhCong.Value.Year == now.Year &&
                            d.NgayGiaoThanhCong.Value.Month == now.Month)
                .SumAsync(d => (decimal?)d.TongThanhToan) ?? 0m;

            var thangTruoc = now.AddMonths(-1);
            decimal doanhThuThangTruoc = await queryHoanThanh
                .Where(d => d.NgayGiaoThanhCong != null &&
                            d.NgayGiaoThanhCong.Value.Year == thangTruoc.Year &&
                            d.NgayGiaoThanhCong.Value.Month == thangTruoc.Month)
                .SumAsync(d => (decimal?)d.TongThanhToan) ?? 0m;

            double? phanTramThayDoi = null;
            if (doanhThuThangTruoc > 0)
            {
                phanTramThayDoi = Math.Round((double)((doanhThuThangHienTai - doanhThuThangTruoc) / doanhThuThangTruoc) * 100, 1);
            }

            int tongDonHang = await _context.DonHang.CountAsync();
            int tongSanPham = await _context.SanPham.CountAsync();
            int soLuongDangKinhDoanh = await _context.SanPham.CountAsync(s => s.TrangThai);
            int soLuongSapHet = await _context.SanPham.CountAsync(s => s.TrangThai && s.SoLuongTon <= s.NguongCanhBao);

            int tongKhachHang = await _context.TaiKhoan.CountAsync(t => t.VaiTro == VaiTroConstant.KhachHang);
            int soKhachHangMoiThangNay = await _context.TaiKhoan.CountAsync(t =>
                t.VaiTro == VaiTroConstant.KhachHang &&
                t.NgayTao.Year == now.Year &&
                t.NgayTao.Month == now.Month);

            // 2. Biểu đồ doanh thu 6 tháng gần nhất
            var bieuDo6Thang = new List<DoanhThuThangItem>();
            var start6Months = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

            var don6Thang = await queryHoanThanh
                .Where(d => d.NgayGiaoThanhCong != null && d.NgayGiaoThanhCong >= start6Months)
                .Select(d => new
                {
                    Year = d.NgayGiaoThanhCong!.Value.Year,
                    Month = d.NgayGiaoThanhCong!.Value.Month,
                    d.TongThanhToan
                })
                .ToListAsync();

            var doanhThuTheoThangGroup = don6Thang
                .GroupBy(d => new { d.Year, d.Month })
                .ToDictionary(g => g.Key, g => g.Sum(x => x.TongThanhToan));

            for (int i = 5; i >= 0; i--)
            {
                var targetDate = now.AddMonths(-i);
                var key = new { Year = targetDate.Year, Month = targetDate.Month };
                decimal dt = doanhThuTheoThangGroup.TryGetValue(key, out var val) ? val : 0m;
                bieuDo6Thang.Add(new DoanhThuThangItem
                {
                    Thang = targetDate.Month,
                    Nam = targetDate.Year,
                    DoanhThu = dt,
                    LaThangHienTai = (targetDate.Year == now.Year && targetDate.Month == now.Month)
                });
            }

            decimal maxDt = bieuDo6Thang.Max(b => b.DoanhThu);
            foreach (var item in bieuDo6Thang)
            {
                if (maxDt > 0 && item.DoanhThu > 0)
                {
                    item.ChieuCaoPhanTram = (int)Math.Clamp(Math.Round((double)(item.DoanhThu / maxDt) * 100), 4, 100);
                }
                else
                {
                    item.ChieuCaoPhanTram = 4;
                }
            }

            // 3. Tỷ lệ trạng thái đơn hàng (5 trạng thái)
            var donHangGroupByTrangThai = await _context.DonHang
                .GroupBy(d => d.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TrangThai, x => x.Count);

            var danhSachTrangThai = new List<TrangThaiDonHangItem>();
            var statuses = new[]
            {
                (TrangThaiDonHangConstant.ChoXacNhan, "text-statusWarning", "bg-statusWarning"),
                (TrangThaiDonHangConstant.DaXacNhan, "text-statusInfo", "bg-statusInfo"),
                (TrangThaiDonHangConstant.DangGiao, "text-primary", "bg-primary"),
                (TrangThaiDonHangConstant.HoanThanh, "text-statusSuccess", "bg-statusSuccess"),
                (TrangThaiDonHangConstant.DaHuy, "text-statusDanger", "bg-statusDanger")
            };

            foreach (var (st, color, bg) in statuses)
            {
                int count = donHangGroupByTrangThai.TryGetValue(st, out var c) ? c : 0;
                double tyLe = tongDonHang > 0 ? Math.Round((double)count / tongDonHang * 100, 1) : 0;
                danhSachTrangThai.Add(new TrangThaiDonHangItem
                {
                    TrangThai = st,
                    TenTrangThai = _locHelper.GetOrderStatusLabel(st),
                    SoLuong = count,
                    TyLePhanTram = tyLe,
                    CssColorClass = color,
                    CssBgClass = bg
                });
            }

            // 4. Top 5 sản phẩm bán chạy
            var topBanChayData = await _context.ChiTietDonHang
                .Where(c => c.MaDHNavigation.TrangThai == TrangThaiDonHangConstant.HoanThanh)
                .GroupBy(c => c.MaSP)
                .Select(g => new
                {
                    MaSP = g.Key,
                    SoLuongBan = g.Sum(c => c.SoLuong),
                    DoanhThu = g.Sum(c => c.ThanhTien ?? (c.DonGia * c.SoLuong))
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(5)
                .ToListAsync();

            var topSpIds = topBanChayData.Select(x => x.MaSP).ToList();
            var spDetails = await _context.SanPham
                .Where(s => topSpIds.Contains(s.MaSP))
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    DuongDanAnh = s.AnhSanPham
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.MaAnh)
                        .Select(a => a.DuongDan)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToDictionaryAsync(s => s.MaSP);

            var topBanChayList = topBanChayData.Select(x =>
            {
                spDetails.TryGetValue(x.MaSP, out var sp);
                return new TopSanPhamBanChayItem
                {
                    MaSP = x.MaSP,
                    TenSP = sp?.TenSP ?? $"SP #{x.MaSP}",
                    DuongDanAnh = sp?.DuongDanAnh ?? string.Empty,
                    Gia = sp?.Gia ?? 0m,
                    SoLuongBan = x.SoLuongBan,
                    DoanhThu = x.DoanhThu
                };
            }).ToList();

            // 5. Danh sách sản phẩm sắp hết hàng (tồn <= ngưỡng cảnh báo, đang kinh doanh)
            var sapHetHangList = await _context.SanPham
                .Where(s => s.TrangThai && s.SoLuongTon <= s.NguongCanhBao)
                .OrderBy(s => s.SoLuongTon)
                .ThenBy(s => s.MaSP)
                .Take(10)
                .Select(s => new SanPhamSapHetHangItem
                {
                    MaSP = s.MaSP,
                    TenSP = s.TenSP,
                    DuongDanAnh = s.AnhSanPham
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.MaAnh)
                        .Select(a => a.DuongDan)
                        .FirstOrDefault() ?? string.Empty,
                    MaDM = s.MaDM,
                    TenDM = s.MaDMNavigation.TenDM,
                    Gia = s.Gia,
                    SoLuongTon = s.SoLuongTon,
                    NguongCanhBao = s.NguongCanhBao,
                    TrangThai = s.TrangThai
                })
                .ToListAsync();

            // 6. Danh sách năm xuất Excel
            var minYearOrder = await queryHoanThanh
                .Where(d => d.NgayGiaoThanhCong != null)
                .Select(d => (int?)d.NgayGiaoThanhCong!.Value.Year)
                .MinAsync();

            int startYear = minYearOrder ?? now.Year;
            if (startYear > now.Year) startYear = now.Year;

            var danhSachNam = Enumerable.Range(startYear, now.Year - startYear + 1)
                .OrderByDescending(y => y)
                .ToList();

            int namDuocChon = nam ?? now.Year;
            if (!danhSachNam.Contains(namDuocChon))
            {
                danhSachNam.Insert(0, namDuocChon);
            }

            return new DashboardViewModel
            {
                TongDoanhThu = tongDoanhThu,
                DoanhThuThangHienTai = doanhThuThangHienTai,
                DoanhThuThangTruoc = doanhThuThangTruoc,
                PhanTramThayDoiDoanhThu = phanTramThayDoi,

                TongDonHang = tongDonHang,
                TongSanPham = tongSanPham,
                SoLuongSanPhamDangKinhDoanh = soLuongDangKinhDoanh,
                SoLuongSanPhamSapHet = soLuongSapHet,

                TongKhachHang = tongKhachHang,
                SoKhachHangMoiThangNay = soKhachHangMoiThangNay,

                BieuDo6Thang = bieuDo6Thang,
                DanhSachTrangThaiDon = danhSachTrangThai,
                TopBanChay = topBanChayList,
                DanhSachSapHetHang = sapHetHangList,

                NamDuocChon = namDuocChon,
                DanhSachNam = danhSachNam
            };
        }

        public async Task<List<DoanhThuThangExcelDto>> LayDoanhThu12ThangAsync(int nam)
        {
            var donTrongNam = await _context.DonHang
                .AsNoTracking()
                .Where(d => d.TrangThai == TrangThaiDonHangConstant.HoanThanh &&
                            d.NgayGiaoThanhCong != null &&
                            d.NgayGiaoThanhCong.Value.Year == nam)
                .Select(d => new
                {
                    Month = d.NgayGiaoThanhCong!.Value.Month,
                    d.TongThanhToan
                })
                .Take(50001)
                .ToListAsync();

            if (donTrongNam.Count > 50000)
            {
                throw new ExcelMaxRowsExceededException();
            }

            var grouped = donTrongNam
                .GroupBy(d => d.Month)
                .ToDictionary(g => g.Key, g => new
                {
                    Count = g.Count(),
                    Revenue = g.Sum(x => x.TongThanhToan)
                });

            var result = new List<DoanhThuThangExcelDto>();
            for (int m = 1; m <= 12; m++)
            {
                grouped.TryGetValue(m, out var val);
                result.Add(new DoanhThuThangExcelDto
                {
                    Thang = m,
                    Nam = nam,
                    SoDonHoanThanh = val?.Count ?? 0,
                    DoanhThu = val?.Revenue ?? 0m
                });
            }

            return result;
        }

        public async Task<List<DonHangHoanThanhThangExcelDto>> LayDonHoanThanhTheoThangAsync(int nam, int thang)
        {
            var query = _context.DonHang
                .AsNoTracking()
                .Include(d => d.MaTKNavigation)
                .Include(d => d.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .Where(d => d.TrangThai == TrangThaiDonHangConstant.HoanThanh &&
                            d.NgayGiaoThanhCong != null &&
                            d.NgayGiaoThanhCong.Value.Year == nam &&
                            d.NgayGiaoThanhCong.Value.Month == thang)
                .OrderBy(d => d.NgayGiaoThanhCong);

            var orders = await query.Take(50001).ToListAsync();
            if (orders.Count > 50000)
            {
                throw new ExcelMaxRowsExceededException();
            }

            var result = orders.Select(d => new DonHangHoanThanhThangExcelDto
            {
                MaDH = d.MaDH,
                MaDonHangText = _locHelper.MaDonHang(d.MaDH),
                TenKhachHang = d.MaTKNavigation?.HoTen ?? string.Empty,
                NgayGiaoThanhCong = d.NgayGiaoThanhCong!.Value,
                TongTienHang = d.TongTienHang,
                CodeKM = !string.IsNullOrWhiteSpace(d.CodeKM) ? d.CodeKM : "-",
                TienGiam = d.TienGiam,
                TongThanhToan = d.TongThanhToan,
                TenPhuongThuc = d.ThanhToan?.MaPTNavigation != null
                    ? _locHelper.GetPaymentMethodLabel(d.ThanhToan.MaPTNavigation.TenPT)
                    : string.Empty
            }).ToList();

            return result;
        }

        public async Task<List<SanPhamTonKhoExcelDto>> LayDuLieuSanPhamTonKhoExcelAsync()
        {
            var query = _context.SanPham
                .AsNoTracking()
                .Include(s => s.MaDMNavigation)
                .Include(s => s.MaTHNavigation)
                .OrderBy(s => s.SoLuongTon)
                .ThenBy(s => s.MaSP);

            var products = await query.Take(50001).ToListAsync();
            if (products.Count > 50000)
            {
                throw new ExcelMaxRowsExceededException();
            }

            var result = products.Select(s =>
            {
                bool sapHet = s.TrangThai && s.SoLuongTon <= s.NguongCanhBao;
                string canhBao;
                if (!s.TrangThai)
                {
                    canhBao = _localizer["Admin_Excel_Status_Inactive"].Value;
                }
                else if (s.SoLuongTon == 0)
                {
                    canhBao = _localizer["Admin_Dashboard_LowStock_OutOfStock"].Value;
                }
                else if (s.SoLuongTon <= s.NguongCanhBao)
                {
                    canhBao = _localizer["Admin_Dashboard_LowStock_LowStock"].Value;
                }
                else
                {
                    canhBao = "-";
                }

                return new SanPhamTonKhoExcelDto
                {
                    MaSP = s.MaSP,
                    TenSP = s.TenSP,
                    TenDM = _locHelper.TenDanhMuc(s.MaDM, s.MaDMNavigation?.TenDM ?? string.Empty),
                    TenTH = s.MaTHNavigation?.TenTH ?? string.Empty,
                    Gia = s.Gia,
                    SoLuongTon = s.SoLuongTon,
                    NguongCanhBao = s.NguongCanhBao,
                    TrangThaiKinhDoanh = s.TrangThai,
                    CanhBao = canhBao,
                    LaSapHet = sapHet
                };
            }).ToList();

            return result;
        }

        public async Task<List<DonHangExcelDto>> LayDuLieuDonHangExcelAsync(
            string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword)
        {
            var query = _context.DonHang
                .AsNoTracking()
                .Include(d => d.MaTKNavigation)
                .Include(d => d.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .AsQueryable();

            // Sử dụng hàm lọc dùng chung theo Điều chỉnh 1
            query = query.LocDonHangAdmin(trangThai, tuNgay, denNgay, keyword);

            var orders = await query
                .OrderByDescending(d => d.NgayDat)
                .Take(50001)
                .ToListAsync();

            if (orders.Count > 50000)
            {
                throw new ExcelMaxRowsExceededException();
            }

            var result = orders.Select(d => new DonHangExcelDto
            {
                MaDH = d.MaDH,
                MaDonHangText = _locHelper.MaDonHang(d.MaDH),
                TenKhachHang = d.MaTKNavigation?.HoTen ?? string.Empty,
                TenNguoiNhan = d.TenNguoiNhan,
                SdtNguoiNhan = d.SdtNguoiNhan,
                NgayDat = d.NgayDat,
                TrangThaiDon = _locHelper.GetOrderStatusLabel(d.TrangThai),
                TenPhuongThuc = d.ThanhToan?.MaPTNavigation != null ? _locHelper.GetPaymentMethodLabel(d.ThanhToan.MaPTNavigation.TenPT) : string.Empty,
                TrangThaiThanhToan = d.ThanhToan != null ? _locHelper.GetPaymentStatusLabel(d.ThanhToan.TrangThai) : string.Empty,
                TongThanhToan = d.TongThanhToan,
                NgayVanDon = d.NgayVanDon,
                NgayGiaoThanhCong = d.NgayGiaoThanhCong
            }).ToList();

            return result;
        }
    }
}
