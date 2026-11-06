using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IKhuyenMaiService _khuyenMaiService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DonHangService> _logger;

        public DonHangService(
            WebsiteBanMyPhamContext context,
            IKhuyenMaiService khuyenMaiService,
            IConfiguration configuration,
            ILogger<DonHangService> logger)
        {
            _context = context;
            _khuyenMaiService = khuyenMaiService;
            _configuration = configuration;
            _logger = logger;
        }

        private class ItemDatHang
        {
            public int MaSP { get; set; }
            public int SoLuong { get; set; }
        }

        public async Task<(bool Success, string? ErrorMessage, int? MaDH)> TaoDonAsync(int maTK, DatHangRequestViewModel request)
        {
            // Bắt đầu một transaction tường minh
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Kiểm tra tài khoản khách hàng
                var user = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTK == maTK);
                if (user == null || !user.TrangThai || user.VaiTro != VaiTroConstant.KhachHang)
                {
                    await transaction.RollbackAsync();
                    return (false, "Tài khoản không hợp lệ hoặc đã bị khóa.", null);
                }

                // Kiểm tra phương thức thanh toán
                var pt = await _context.PhuongThucThanhToan.AsNoTracking().FirstOrDefaultAsync(p => p.MaPT == request.MaPT);
                if (pt == null || !pt.TrangThai)
                {
                    await transaction.RollbackAsync();
                    return (false, "Phương thức thanh toán đã chọn không khả dụng.", null);
                }

                // 2. Thu thập danh sách sản phẩm và số lượng đặt theo nguồn
                var danhSachDat = new List<ItemDatHang>();

                if (request.NguonDat == NguonDatHangConstant.TuGio)
                {
                    if (request.DanhSachMaGH == null || !request.DanhSachMaGH.Any())
                    {
                        await transaction.RollbackAsync();
                        return (false, "Vui lòng chọn ít nhất một sản phẩm từ giỏ hàng để đặt.", null);
                    }

                    var distinctMaGH = request.DanhSachMaGH.Distinct().ToList();

                    // Đọc từ giỏ hàng theo MaGH VÀ MaTK (bảo đảm quyền sở hữu tuyệt đối)
                    var dongGios = await _context.GioHang
                        .AsNoTracking()
                        .Where(g => g.MaTK == maTK && distinctMaGH.Contains(g.MaGH))
                        .ToListAsync();

                    if (dongGios.Count != distinctMaGH.Count)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Danh sách giỏ hàng không hợp lệ hoặc có sản phẩm không thuộc quyền sở hữu của bạn.", null);
                    }

                    danhSachDat = dongGios.Select(g => new ItemDatHang
                    {
                        MaSP = g.MaSP,
                        SoLuong = g.SoLuong
                    }).ToList();
                }
                else if (request.NguonDat == NguonDatHangConstant.MuaNgay)
                {
                    if (!request.MaSPMuaNgay.HasValue || request.SoLuongMuaNgay <= 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Thông tin sản phẩm hoặc số lượng mua ngay không hợp lệ.", null);
                    }

                    danhSachDat.Add(new ItemDatHang
                    {
                        MaSP = request.MaSPMuaNgay.Value,
                        SoLuong = request.SoLuongMuaNgay
                    });
                }
                else
                {
                    await transaction.RollbackAsync();
                    return (false, "Nguồn đặt hàng không hợp lệ.", null);
                }

                // Gom các dòng trùng MaSP nếu có
                var danhSachGom = danhSachDat
                    .GroupBy(d => d.MaSP)
                    .Select(g => new ItemDatHang
                    {
                        MaSP = g.Key,
                        SoLuong = g.Sum(x => x.SoLuong)
                    })
                    // Sắp xếp theo MaSP tăng dần để chống deadlock khi cập nhật kho
                    .OrderBy(x => x.MaSP)
                    .ToList();

                var listMaSP = danhSachGom.Select(d => d.MaSP).ToList();

                // Đọc thông tin sản phẩm mới nhất từ DB bằng AsNoTracking
                var spDbList = await _context.SanPham
                    .AsNoTracking()
                    .Where(s => listMaSP.Contains(s.MaSP))
                    .ToListAsync();

                if (spDbList.Count != listMaSP.Count)
                {
                    await transaction.RollbackAsync();
                    return (false, "Một số sản phẩm không tồn tại trong hệ thống.", null);
                }

                decimal tongTienHang = 0;
                var danhSachSanPhamDat = new List<(SanPham Sp, int SoLuong)>();

                foreach (var item in danhSachGom)
                {
                    var sp = spDbList.First(s => s.MaSP == item.MaSP);

                    if (!sp.TrangThai)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Sản phẩm '{sp.TenSP}' hiện đã ngừng kinh doanh.", null);
                    }

                    if (sp.SoLuongTon <= 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Sản phẩm '{sp.TenSP}' hiện đã hết hàng.", null);
                    }

                    if (sp.SoLuongTon < item.SoLuong)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Sản phẩm '{sp.TenSP}' không đủ số lượng trong kho (chỉ còn {sp.SoLuongTon}).", null);
                    }

                    tongTienHang += sp.Gia * item.SoLuong;
                    danhSachSanPhamDat.Add((sp, item.SoLuong));
                }

                // 3. Xử lý mã khuyến mãi nếu có
                KhuyenMaiKetQuaViewModel? kmKetQua = null;
                decimal tienGiam = 0;

                if (!string.IsNullOrWhiteSpace(request.CodeKM))
                {
                    kmKetQua = await _khuyenMaiService.TinhGiamAsync(request.CodeKM, tongTienHang);
                    if (!kmKetQua.HopLe)
                    {
                        await transaction.RollbackAsync();
                        return (false, kmKetQua.ThongBao ?? "Mã khuyến mãi không hợp lệ.", null);
                    }

                    tienGiam = kmKetQua.TienGiam;

                    // Tăng số lượt đã dùng của mã KM bằng câu UPDATE nguyên tử có điều kiện
                    var affectedKM = await _context.KhuyenMai
                        .Where(k => k.MaKM == kmKetQua.MaKM!.Value 
                                    && k.TrangThai 
                                    && DateTime.Now >= k.NgayBatDau 
                                    && DateTime.Now <= k.NgayKetThuc 
                                    && (k.SoLuotToiDa == null || k.SoLuotDaDung < k.SoLuotToiDa))
                        .ExecuteUpdateAsync(s => s.SetProperty(k => k.SoLuotDaDung, k => k.SoLuotDaDung + 1));

                    if (affectedKM == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Mã khuyến mãi đã hết lượt sử dụng hoặc vừa hết hạn. Vui lòng thử lại.", null);
                    }
                }

                decimal tongThanhToan = tongTienHang - tienGiam;

                // 4. Trừ tồn kho nguyên tử cho từng sản phẩm (theo thứ tự MaSP tăng dần để tránh deadlock)
                foreach (var (sp, soLuong) in danhSachSanPhamDat)
                {
                    var affectedSP = await _context.SanPham
                        .Where(s => s.MaSP == sp.MaSP && s.TrangThai && s.SoLuongTon >= soLuong)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.SoLuongTon, p => p.SoLuongTon - soLuong));

                    if (affectedSP == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Sản phẩm '{sp.TenSP}' không đủ số lượng tồn kho hoặc đã ngừng kinh doanh khi xử lý.", null);
                    }
                }

                // 5. Tạo đơn hàng
                var donHang = new DonHang
                {
                    MaTK = maTK,
                    NgayDat = DateTime.Now,
                    TenNguoiNhan = request.TenNguoiNhan.Trim(),
                    SdtNguoiNhan = request.SdtNguoiNhan.Trim(),
                    DiaChiGiao = request.DiaChiGiao.Trim(),
                    GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim(),
                    TongTienHang = tongTienHang,
                    MaKM = kmKetQua?.MaKM,
                    CodeKM = kmKetQua?.Code,
                    TienGiam = tienGiam,
                    TongThanhToan = tongThanhToan,
                    TrangThai = TrangThaiDonHangConstant.ChoXacNhan,
                    NgayVanDon = null,
                    NgayGiaoThanhCong = null,
                    MaVanDon = null
                };

                _context.DonHang.Add(donHang);
                await _context.SaveChangesAsync();

                // 6. Tạo chi tiết đơn hàng (lưu TenSP và DonGia tại thời điểm mua; KHÔNG gán ThanhTien vì là computed column)
                foreach (var (sp, soLuong) in danhSachSanPhamDat)
                {
                    var ctdh = new ChiTietDonHang
                    {
                        MaDH = donHang.MaDH,
                        MaSP = sp.MaSP,
                        TenSP = sp.TenSP,
                        DonGia = sp.Gia,
                        SoLuong = soLuong
                    };
                    _context.ChiTietDonHang.Add(ctdh);
                }

                // 7. Tạo bản ghi ThanhToan
                var thanhToan = new ThanhToan
                {
                    MaDH = donHang.MaDH,
                    MaPT = request.MaPT,
                    SoTien = tongThanhToan,
                    TrangThai = TrangThaiThanhToanConstant.ChuaThanhToan,
                    TrangThaiHoanTien = TrangThaiHoanTienConstant.KhongCan,
                    NgayTao = DateTime.Now
                };
                _context.ThanhToan.Add(thanhToan);

                // 8. Tạo bản ghi LichSuDonHang
                var lichSu = new LichSuDonHang
                {
                    MaDH = donHang.MaDH,
                    TrangThaiCu = null,
                    TrangThaiMoi = TrangThaiDonHangConstant.ChoXacNhan,
                    MaTKThucHien = maTK,
                    NgayDoi = DateTime.Now
                };
                _context.LichSuDonHang.Add(lichSu);

                await _context.SaveChangesAsync();

                // 9. Nếu nguồn đặt là TuGio: chỉ xóa các dòng giỏ hàng đã đặt của khách hàng
                if (request.NguonDat == NguonDatHangConstant.TuGio)
                {
                    var maGHDaDat = request.DanhSachMaGH.Distinct().ToList();
                    var cacDongGioXoa = await _context.GioHang
                        .Where(g => g.MaTK == maTK && maGHDaDat.Contains(g.MaGH))
                        .ToListAsync();

                    _context.GioHang.RemoveRange(cacDongGioXoa);
                    await _context.SaveChangesAsync();
                }

                // 10. Commit transaction
                await transaction.CommitAsync();

                return (true, null, donHang.MaDH);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình tạo đơn hàng cho MaTK {MaTK}", maTK);
                return (false, "Đã xảy ra lỗi trong quá trình đặt hàng. Dữ liệu của bạn chưa bị thay đổi, vui lòng thử lại.", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> HuyDonAsync(int maTK, int maDH)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Cập nhật trạng thái đơn hàng bằng UPDATE nguyên tử có điều kiện
                var affected = await _context.DonHang
                    .Where(d => d.MaDH == maDH && d.MaTK == maTK && d.TrangThai == TrangThaiDonHangConstant.ChoXacNhan)
                    .ExecuteUpdateAsync(s => s.SetProperty(d => d.TrangThai, TrangThaiDonHangConstant.DaHuy));

                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, "Không thể hủy đơn hàng (đơn không thuộc về bạn, không ở trạng thái Chờ xác nhận hoặc đã được hủy trước đó).");
                }

                // Thứ tự cập nhật tài nguyên: KhuyenMai trước, SanPham sau để thống nhất thứ tự khóa với TaoDonAsync, tránh deadlock
                // 1. Hoàn lại 1 lượt dùng mã khuyến mãi nếu đơn có sử dụng mã
                var donHang = await _context.DonHang.AsNoTracking().FirstOrDefaultAsync(d => d.MaDH == maDH);
                if (donHang?.MaKM != null)
                {
                    await _context.KhuyenMai
                        .Where(k => k.MaKM == donHang.MaKM.Value && k.SoLuotDaDung > 0)
                        .ExecuteUpdateAsync(s => s.SetProperty(k => k.SoLuotDaDung, k => k.SoLuotDaDung - 1));
                }

                // 2. Lấy chi tiết đơn hàng để hoàn tồn kho, sắp xếp MaSP tăng dần để tránh deadlock
                var chiTiets = await _context.ChiTietDonHang
                    .AsNoTracking()
                    .Where(c => c.MaDH == maDH)
                    .OrderBy(c => c.MaSP)
                    .ToListAsync();

                foreach (var ct in chiTiets)
                {
                    await _context.SanPham
                        .Where(s => s.MaSP == ct.MaSP)
                        .ExecuteUpdateAsync(s => s.SetProperty(sp => sp.SoLuongTon, sp => sp.SoLuongTon + ct.SoLuong));
                }

                // 3. Nếu đơn Online đã thanh toán thành công, chuyển sang ChoHoan
                var thanhToan = await _context.ThanhToan.FirstOrDefaultAsync(t => t.MaDH == maDH);
                if (thanhToan != null && thanhToan.TrangThai == TrangThaiThanhToanConstant.DaThanhToan)
                {
                    thanhToan.TrangThaiHoanTien = TrangThaiHoanTienConstant.ChoHoan;
                }

                // 4. Ghi nhận lịch sử đơn hàng
                var lichSu = new LichSuDonHang
                {
                    MaDH = maDH,
                    TrangThaiCu = TrangThaiDonHangConstant.ChoXacNhan,
                    TrangThaiMoi = TrangThaiDonHangConstant.DaHuy,
                    MaTKThucHien = maTK,
                    NgayDoi = DateTime.Now
                };
                _context.LichSuDonHang.Add(lichSu);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi hủy đơn hàng MaDH {MaDH} bởi MaTK {MaTK}", maDH, maTK);
                return (false, "Đã xảy ra lỗi khi hủy đơn hàng. Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng theo luồng hợp lệ. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int maTKAdmin, int maDH, string trangThaiMoi, string? maVanDon = null)
        {
            // Trim và kiểm tra độ dài mã vận đơn trước khi vào transaction
            maVanDon = string.IsNullOrWhiteSpace(maVanDon) ? null : maVanDon.Trim();
            if (maVanDon != null && maVanDon.Length > 50)
            {
                return (false, "Mã vận đơn tối đa 50 ký tự.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kiểm tra tài khoản Admin hợp lệ
                var admin = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTK == maTKAdmin);
                if (admin == null || !admin.TrangThai || admin.VaiTro != VaiTroConstant.QuanTriVien)
                {
                    await transaction.RollbackAsync();
                    return (false, "Bạn không có quyền thực hiện thao tác quản trị này.");
                }

                // Admin tuyệt đối không được chuyển đơn sang DaHuy
                if (trangThaiMoi == TrangThaiDonHangConstant.DaHuy)
                {
                    await transaction.RollbackAsync();
                    return (false, "Admin không thể chuyển trạng thái đơn hàng sang Đã hủy.");
                }

                var donHang = await _context.DonHang
                    .Include(d => d.ThanhToan)
                        .ThenInclude(t => t!.MaPTNavigation)
                    .FirstOrDefaultAsync(d => d.MaDH == maDH);

                if (donHang == null)
                {
                    await transaction.RollbackAsync();
                    return (false, "Không tìm thấy đơn hàng.");
                }

                var trangThaiHienTai = donHang.TrangThai;

                // Kiểm tra tính hợp lệ của luồng trạng thái
                bool hopLe = false;
                if (trangThaiHienTai == TrangThaiDonHangConstant.ChoXacNhan && trangThaiMoi == TrangThaiDonHangConstant.DaXacNhan)
                {
                    // Chính sách đơn Online: Không cho chuyển sang DaXacNhan nếu chưa thanh toán
                    var tenPT = donHang.ThanhToan?.MaPTNavigation?.TenPT;
                    if (tenPT == PhuongThucThanhToanConstant.Online && donHang.ThanhToan?.TrangThai != TrangThaiThanhToanConstant.DaThanhToan)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Đơn hàng thanh toán trực tuyến chưa hoàn tất thanh toán, không thể xác nhận đơn.");
                    }
                    hopLe = true;
                }
                else if (trangThaiHienTai == TrangThaiDonHangConstant.DaXacNhan && trangThaiMoi == TrangThaiDonHangConstant.DangGiao)
                {
                    hopLe = true;
                }
                else if (trangThaiHienTai == TrangThaiDonHangConstant.DangGiao && trangThaiMoi == TrangThaiDonHangConstant.HoanThanh)
                {
                    hopLe = true;
                }

                if (!hopLe)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Không thể chuyển từ trạng thái '{trangThaiHienTai}' sang '{trangThaiMoi}'. Chỉ cho phép chuyển sang bước kế tiếp hợp lệ.");
                }

                var now = DateTime.Now;

                // Cập nhật trạng thái và ngày tương ứng trong cùng một lần lưu để tuân thủ CK_DH_TrangThai_Ngay
                if (trangThaiMoi == TrangThaiDonHangConstant.DaXacNhan)
                {
                    var affected = await _context.DonHang
                        .Where(d => d.MaDH == maDH && d.TrangThai == TrangThaiDonHangConstant.ChoXacNhan)
                        .ExecuteUpdateAsync(s => s.SetProperty(d => d.TrangThai, TrangThaiDonHangConstant.DaXacNhan));

                    if (affected == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Đơn hàng đã bị thay đổi trạng thái bởi một phiên khác.");
                    }
                }
                else if (trangThaiMoi == TrangThaiDonHangConstant.DangGiao)
                {
                    int affected;
                    if (!string.IsNullOrEmpty(maVanDon))
                    {
                        affected = await _context.DonHang
                            .Where(d => d.MaDH == maDH && d.TrangThai == TrangThaiDonHangConstant.DaXacNhan)
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(d => d.TrangThai, TrangThaiDonHangConstant.DangGiao)
                                .SetProperty(d => d.NgayVanDon, now)
                                .SetProperty(d => d.MaVanDon, maVanDon));
                    }
                    else
                    {
                        affected = await _context.DonHang
                            .Where(d => d.MaDH == maDH && d.TrangThai == TrangThaiDonHangConstant.DaXacNhan)
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(d => d.TrangThai, TrangThaiDonHangConstant.DangGiao)
                                .SetProperty(d => d.NgayVanDon, now));
                    }

                    if (affected == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Đơn hàng đã bị thay đổi trạng thái bởi một phiên khác.");
                    }
                }
                else if (trangThaiMoi == TrangThaiDonHangConstant.HoanThanh)
                {
                    var affected = await _context.DonHang
                        .Where(d => d.MaDH == maDH && d.TrangThai == TrangThaiDonHangConstant.DangGiao)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(d => d.TrangThai, TrangThaiDonHangConstant.HoanThanh)
                            .SetProperty(d => d.NgayGiaoThanhCong, now));

                    if (affected == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Đơn hàng đã bị thay đổi trạng thái bởi một phiên khác.");
                    }

                    // Nếu phương thức thanh toán là COD thì chuyển sang DaThanhToan và ghi NgayThanhToan
                    var tenPT = donHang.ThanhToan?.MaPTNavigation?.TenPT;
                    if (tenPT == PhuongThucThanhToanConstant.COD)
                    {
                        await _context.ThanhToan
                            .Where(t => t.MaDH == maDH && t.TrangThai != TrangThaiThanhToanConstant.DaThanhToan)
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(t => t.TrangThai, TrangThaiThanhToanConstant.DaThanhToan)
                                .SetProperty(t => t.NgayThanhToan, now));
                    }
                }

                // Ghi nhận lịch sử đơn hàng
                var lichSu = new LichSuDonHang
                {
                    MaDH = maDH,
                    TrangThaiCu = trangThaiHienTai,
                    TrangThaiMoi = trangThaiMoi,
                    MaTKThucHien = maTKAdmin,
                    NgayDoi = now
                };
                _context.LichSuDonHang.Add(lichSu);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi cập nhật trạng thái đơn hàng MaDH {MaDH} bởi Admin {MaTKAdmin}", maDH, maTKAdmin);
                return (false, "Đã xảy ra lỗi khi cập nhật trạng thái đơn hàng.");
            }
        }

        /// <summary>
        /// Xác nhận hoàn tiền cho đơn hàng. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> XacNhanHoanTienAsync(int maTKAdmin, int maDH)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var admin = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTK == maTKAdmin);
                if (admin == null || !admin.TrangThai || admin.VaiTro != VaiTroConstant.QuanTriVien)
                {
                    await transaction.RollbackAsync();
                    return (false, "Bạn không có quyền thực hiện thao tác hoàn tiền.");
                }

                var now = DateTime.Now;

                // Cập nhật ThanhToan có TrangThai = DaThanhToan và TrangThaiHoanTien = ChoHoan sang DaHoan
                var affected = await _context.ThanhToan
                    .Where(t => t.MaDH == maDH 
                                && t.TrangThai == TrangThaiThanhToanConstant.DaThanhToan 
                                && t.TrangThaiHoanTien == TrangThaiHoanTienConstant.ChoHoan)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(t => t.TrangThaiHoanTien, TrangThaiHoanTienConstant.DaHoan)
                        .SetProperty(t => t.NgayHoanTien, now));

                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, "Không tìm thấy giao dịch cần hoàn tiền hoặc giao dịch chưa ở trạng thái Chờ hoàn.");
                }

                await transaction.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi xác nhận hoàn tiền cho đơn hàng MaDH {MaDH}", maDH);
                return (false, "Đã xảy ra lỗi khi xác nhận hoàn tiền.");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> GhiNhanThanhToanOnlineMoPhongAsync(int maTK, int maDH, bool thanhCong)
        {
            // Kiểm tra cấu hình ChoPhepMoPhong
            bool choPhep = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");
            if (!choPhep)
            {
                return (false, "Chức năng thanh toán trực tuyến mô phỏng hiện đang bị tắt trên hệ thống.");
            }

            var donHang = await _context.DonHang
                .Include(d => d.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .FirstOrDefaultAsync(d => d.MaDH == maDH && d.MaTK == maTK);

            if (donHang == null)
            {
                return (false, "Không tìm thấy đơn hàng hoặc đơn không thuộc về bạn.");
            }

            if (donHang.TrangThai == TrangThaiDonHangConstant.DaHuy)
            {
                return (false, "Đơn hàng này đã bị hủy, không thể thực hiện thanh toán.");
            }

            var tenPT = donHang.ThanhToan?.MaPTNavigation?.TenPT;
            if (tenPT != PhuongThucThanhToanConstant.Online)
            {
                return (false, "Đơn hàng này không sử dụng phương thức thanh toán trực tuyến.");
            }

            var now = DateTime.Now;

            if (thanhCong)
            {
                var maGiaoDich = "MOPHON-" + RandomNumberGenerator.GetInt32(100000, 999999);

                // Đưa điều kiện đơn chưa hủy vào ngay trong câu ExecuteUpdateAsync
                var affected = await _context.ThanhToan
                    .Where(t => t.MaDH == maDH 
                                && _context.DonHang.Any(d => d.MaDH == t.MaDH && d.TrangThai != TrangThaiDonHangConstant.DaHuy)
                                && (t.TrangThai == TrangThaiThanhToanConstant.ChuaThanhToan || t.TrangThai == TrangThaiThanhToanConstant.ThatBai))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(t => t.TrangThai, TrangThaiThanhToanConstant.DaThanhToan)
                        .SetProperty(t => t.MaGiaoDich, maGiaoDich)
                        .SetProperty(t => t.NgayThanhToan, now));

                if (affected == 0)
                {
                    return (false, "Đơn hàng đã bị hủy hoặc đã được thanh toán trước đó.");
                }
            }
            else
            {
                // Đưa điều kiện đơn chưa hủy vào ngay trong câu ExecuteUpdateAsync
                var affected = await _context.ThanhToan
                    .Where(t => t.MaDH == maDH 
                                && _context.DonHang.Any(d => d.MaDH == t.MaDH && d.TrangThai != TrangThaiDonHangConstant.DaHuy)
                                && t.TrangThai == TrangThaiThanhToanConstant.ChuaThanhToan)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.TrangThai, TrangThaiThanhToanConstant.ThatBai));

                if (affected == 0)
                {
                    return (false, "Đơn hàng đã bị hủy hoặc đã được thanh toán trước đó.");
                }
            }

            return (true, null);
        }

        public async Task<DonHangKhachListViewModel> LayDonHangCuaKhachAsync(int maTK, string? trangThai, int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.DonHang
                .AsNoTracking()
                .Where(d => d.MaTK == maTK);

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai.Trim());
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderByDescending(d => d.NgayDat)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DonHangKhachItemViewModel
                {
                    MaDH = d.MaDH,
                    NgayDat = d.NgayDat,
                    TongTienHang = d.TongTienHang,
                    TienGiam = d.TienGiam,
                    TongThanhToan = d.TongThanhToan,
                    TrangThai = d.TrangThai,
                    TenPhuongThuc = d.ThanhToan != null ? d.ThanhToan.MaPTNavigation.TenPT : "",
                    SoLuongSanPham = d.ChiTietDonHang.Sum(c => c.SoLuong),
                    SanPham = d.ChiTietDonHang.Select(c => new ChiTietSanPhamDonHangViewModel
                    {
                        MaCTDH = c.MaCTDH,
                        MaSP = c.MaSP,
                        TenSP = c.TenSP,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong,
                        DuongDanAnh = c.MaSPNavigation.AnhSanPham.Where(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                                      ?? c.MaSPNavigation.AnhSanPham.Select(a => a.DuongDan).FirstOrDefault(),
                        DaDanhGia = c.DanhGia != null
                    }).ToList()
                })
                .ToListAsync();

            return new DonHangKhachListViewModel
            {
                DonHangs = items,
                CurrentTrangThai = trangThai,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<DonHangKhachChiTietViewModel?> LayChiTietDonHangKhachAsync(int maTK, int maDH)
        {
            var d = await _context.DonHang
                .AsNoTracking()
                .Where(x => x.MaDH == maDH && x.MaTK == maTK)
                .Include(x => x.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .Include(x => x.ChiTietDonHang)
                    .ThenInclude(c => c.MaSPNavigation)
                        .ThenInclude(sp => sp.AnhSanPham)
                .Include(x => x.ChiTietDonHang)
                    .ThenInclude(c => c.DanhGia)
                .Include(x => x.LichSuDonHang)
                    .ThenInclude(ls => ls.MaTKThucHienNavigation)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (d == null) return null;

            return new DonHangKhachChiTietViewModel
            {
                MaDH = d.MaDH,
                MaTK = d.MaTK,
                NgayDat = d.NgayDat,
                TenNguoiNhan = d.TenNguoiNhan,
                SdtNguoiNhan = d.SdtNguoiNhan,
                DiaChiGiao = d.DiaChiGiao,
                GhiChu = d.GhiChu,
                TongTienHang = d.TongTienHang,
                CodeKM = d.CodeKM,
                TienGiam = d.TienGiam,
                TongThanhToan = d.TongThanhToan,
                TrangThai = d.TrangThai,
                NgayVanDon = d.NgayVanDon,
                NgayGiaoThanhCong = d.NgayGiaoThanhCong,
                MaVanDon = d.MaVanDon,
                ThanhToan = d.ThanhToan == null ? null : new ThanhToanInfoViewModel
                {
                    MaTT = d.ThanhToan.MaTT,
                    MaPT = d.ThanhToan.MaPT,
                    TenPT = d.ThanhToan.MaPTNavigation.TenPT,
                    SoTien = d.ThanhToan.SoTien,
                    TrangThai = d.ThanhToan.TrangThai,
                    TrangThaiHoanTien = d.ThanhToan.TrangThaiHoanTien,
                    MaGiaoDich = d.ThanhToan.MaGiaoDich,
                    NgayThanhToan = d.ThanhToan.NgayThanhToan,
                    NgayHoanTien = d.ThanhToan.NgayHoanTien
                },
                ChiTiet = d.ChiTietDonHang.Select(c => new ChiTietSanPhamDonHangViewModel
                {
                    MaCTDH = c.MaCTDH,
                    MaSP = c.MaSP,
                    TenSP = c.TenSP,
                    DonGia = c.DonGia,
                    SoLuong = c.SoLuong,
                    DuongDanAnh = c.MaSPNavigation.AnhSanPham.Where(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                                  ?? c.MaSPNavigation.AnhSanPham.Select(a => a.DuongDan).FirstOrDefault(),
                    DaDanhGia = c.DanhGia != null
                }).ToList(),
                LichSu = d.LichSuDonHang.OrderBy(ls => ls.NgayDoi).Select(ls => new LichSuDonHangItemViewModel
                {
                    MaLS = ls.MaLS,
                    TrangThaiCu = ls.TrangThaiCu,
                    TrangThaiMoi = ls.TrangThaiMoi,
                    NguoiThucHien = ls.MaTKThucHienNavigation?.HoTen,
                    NgayDoi = ls.NgayDoi
                }).ToList()
            };
        }

        /// <summary>
        /// Lấy danh sách đơn hàng cho Admin. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        public async Task<DonHangAdminListViewModel> LayDonHangAdminAsync(string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.DonHang
                .AsNoTracking()
                .Include(d => d.MaTKNavigation)
                .Include(d => d.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai.Trim());
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayDat >= tuNgay.Value.Date);
            }

            if (denNgay.HasValue)
            {
                var endDay = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDat <= endDay);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(d => d.MaDH.ToString().Contains(kw) 
                                         || d.TenNguoiNhan.ToLower().Contains(kw) 
                                         || d.SdtNguoiNhan.Contains(kw) 
                                         || d.MaTKNavigation.HoTen.ToLower().Contains(kw));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderByDescending(d => d.NgayDat)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DonHangAdminItemViewModel
                {
                    MaDH = d.MaDH,
                    MaTK = d.MaTK,
                    TenKhachHang = d.MaTKNavigation.HoTen,
                    EmailKhachHang = d.MaTKNavigation.Email,
                    TenNguoiNhan = d.TenNguoiNhan,
                    SdtNguoiNhan = d.SdtNguoiNhan,
                    NgayDat = d.NgayDat,
                    TongThanhToan = d.TongThanhToan,
                    TrangThai = d.TrangThai,
                    TenPhuongThuc = d.ThanhToan != null ? d.ThanhToan.MaPTNavigation.TenPT : "",
                    TrangThaiThanhToan = d.ThanhToan != null ? d.ThanhToan.TrangThai : "",
                    TrangThaiHoanTien = d.ThanhToan != null ? d.ThanhToan.TrangThaiHoanTien : "",
                    NgayVanDon = d.NgayVanDon,
                    NgayGiaoThanhCong = d.NgayGiaoThanhCong
                })
                .ToListAsync();

            return new DonHangAdminListViewModel
            {
                DonHangs = items,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                Keyword = keyword,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng cho Admin. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        public async Task<DonHangAdminChiTietViewModel?> LayChiTietDonHangAdminAsync(int maDH)
        {
            var d = await _context.DonHang
                .AsNoTracking()
                .Where(x => x.MaDH == maDH)
                .Include(x => x.MaTKNavigation)
                .Include(x => x.ThanhToan)
                    .ThenInclude(t => t!.MaPTNavigation)
                .Include(x => x.ChiTietDonHang)
                    .ThenInclude(c => c.MaSPNavigation)
                        .ThenInclude(sp => sp.AnhSanPham)
                .Include(x => x.ChiTietDonHang)
                    .ThenInclude(c => c.DanhGia)
                .Include(x => x.LichSuDonHang)
                    .ThenInclude(ls => ls.MaTKThucHienNavigation)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (d == null) return null;

            return new DonHangAdminChiTietViewModel
            {
                MaDH = d.MaDH,
                MaTK = d.MaTK,
                TenKhachHang = d.MaTKNavigation.HoTen,
                EmailKhachHang = d.MaTKNavigation.Email,
                NgayDat = d.NgayDat,
                TenNguoiNhan = d.TenNguoiNhan,
                SdtNguoiNhan = d.SdtNguoiNhan,
                DiaChiGiao = d.DiaChiGiao,
                GhiChu = d.GhiChu,
                TongTienHang = d.TongTienHang,
                CodeKM = d.CodeKM,
                TienGiam = d.TienGiam,
                TongThanhToan = d.TongThanhToan,
                TrangThai = d.TrangThai,
                NgayVanDon = d.NgayVanDon,
                NgayGiaoThanhCong = d.NgayGiaoThanhCong,
                MaVanDon = d.MaVanDon,
                ThanhToan = d.ThanhToan == null ? null : new ThanhToanInfoViewModel
                {
                    MaTT = d.ThanhToan.MaTT,
                    MaPT = d.ThanhToan.MaPT,
                    TenPT = d.ThanhToan.MaPTNavigation.TenPT,
                    SoTien = d.ThanhToan.SoTien,
                    TrangThai = d.ThanhToan.TrangThai,
                    TrangThaiHoanTien = d.ThanhToan.TrangThaiHoanTien,
                    MaGiaoDich = d.ThanhToan.MaGiaoDich,
                    NgayThanhToan = d.ThanhToan.NgayThanhToan,
                    NgayHoanTien = d.ThanhToan.NgayHoanTien
                },
                ChiTiet = d.ChiTietDonHang.Select(c => new ChiTietSanPhamDonHangViewModel
                {
                    MaCTDH = c.MaCTDH,
                    MaSP = c.MaSP,
                    TenSP = c.TenSP,
                    DonGia = c.DonGia,
                    SoLuong = c.SoLuong,
                    DuongDanAnh = c.MaSPNavigation.AnhSanPham.Where(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                                  ?? c.MaSPNavigation.AnhSanPham.Select(a => a.DuongDan).FirstOrDefault(),
                    DaDanhGia = c.DanhGia != null
                }).ToList(),
                LichSu = d.LichSuDonHang.OrderBy(ls => ls.NgayDoi).Select(ls => new LichSuDonHangItemViewModel
                {
                    MaLS = ls.MaLS,
                    TrangThaiCu = ls.TrangThaiCu,
                    TrangThaiMoi = ls.TrangThaiMoi,
                    NguoiThucHien = ls.MaTKThucHienNavigation?.HoTen,
                    NgayDoi = ls.NgayDoi
                }).ToList()
            };
        }
    }
}
