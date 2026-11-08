using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class DiaChiService : IDiaChiService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<DiaChiService> _logger;

        public DiaChiService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer,
            ILogger<DiaChiService> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<List<DiaChiItemViewModel>> LayDanhSachDiaChiAsync(int maTK)
        {
            return await _context.DiaChiNhanHang
                .AsNoTracking()
                .Where(d => d.MaTK == maTK)
                .OrderByDescending(d => d.LaMacDinh)
                .ThenByDescending(d => d.NgayTao)
                .ThenByDescending(d => d.MaDC)
                .Select(d => new DiaChiItemViewModel
                {
                    MaDC = d.MaDC,
                    TenNguoiNhan = d.TenNguoiNhan,
                    SoDienThoai = d.SoDienThoai,
                    DiaChi = d.DiaChi,
                    LaMacDinh = d.LaMacDinh,
                    NgayTao = d.NgayTao
                })
                .ToListAsync();
        }

        public async Task<DiaChiItemViewModel?> LayChiTietAsync(int maDC, int maTK)
        {
            return await _context.DiaChiNhanHang
                .AsNoTracking()
                .Where(d => d.MaDC == maDC && d.MaTK == maTK)
                .Select(d => new DiaChiItemViewModel
                {
                    MaDC = d.MaDC,
                    TenNguoiNhan = d.TenNguoiNhan,
                    SoDienThoai = d.SoDienThoai,
                    DiaChi = d.DiaChi,
                    LaMacDinh = d.LaMacDinh,
                    NgayTao = d.NgayTao
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool HopLe, string? ThongBaoLoi, int? MaDCMoi)> ThemDiaChiAsync(int maTK, DiaChiFormViewModel model)
        {
            var currentCount = await _context.DiaChiNhanHang.CountAsync(d => d.MaTK == maTK);
            if (currentCount >= DiaChiConstant.ToiDaDiaChi)
            {
                return (false, _localizer["Msg_Address_LimitExceeded"].Value, null);
            }

            var ten = model.TenNguoiNhan?.Trim() ?? string.Empty;
            var sdt = model.SoDienThoai?.Trim() ?? string.Empty;
            var diaChi = model.DiaChi?.Trim() ?? string.Empty;

            bool laMacDinh = currentCount == 0 || model.LaMacDinh;

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (laMacDinh)
                {
                    await _context.DiaChiNhanHang
                        .Where(d => d.MaTK == maTK && d.LaMacDinh)
                        .ExecuteUpdateAsync(s => s.SetProperty(d => d.LaMacDinh, false));
                }

                var entity = new DiaChiNhanHang
                {
                    MaTK = maTK,
                    TenNguoiNhan = ten,
                    SoDienThoai = sdt,
                    DiaChi = diaChi,
                    LaMacDinh = laMacDinh,
                    NgayTao = DateTime.Now
                };

                _context.DiaChiNhanHang.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null, entity.MaDC);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật CSDL khi thêm địa chỉ cho tài khoản {MaTK}", maTK);
                return (false, _localizer["Common_Error"].Value, null);
            }
        }

        public async Task<(bool HopLe, string? ThongBaoLoi)> SuaDiaChiAsync(int maTK, int maDC, DiaChiFormViewModel model)
        {
            var entity = await _context.DiaChiNhanHang.FirstOrDefaultAsync(d => d.MaDC == maDC && d.MaTK == maTK);
            if (entity == null)
            {
                return (false, _localizer["Msg_Address_NotFound"].Value);
            }

            entity.TenNguoiNhan = model.TenNguoiNhan?.Trim() ?? string.Empty;
            entity.SoDienThoai = model.SoDienThoai?.Trim() ?? string.Empty;
            entity.DiaChi = model.DiaChi?.Trim() ?? string.Empty;

            try
            {
                // Nếu địa chỉ chưa phải mặc định nhưng người dùng chọn đặt làm mặc định
                if (!entity.LaMacDinh && model.LaMacDinh)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    await _context.DiaChiNhanHang
                        .Where(d => d.MaTK == maTK && d.LaMacDinh)
                        .ExecuteUpdateAsync(s => s.SetProperty(d => d.LaMacDinh, false));

                    entity.LaMacDinh = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    // Nếu đang là mặc định sẵn -> luôn giữ nguyên mặc định (server bỏ qua yêu cầu bỏ mặc định)
                    await _context.SaveChangesAsync();
                }

                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật CSDL khi sửa địa chỉ {MaDC} cho tài khoản {MaTK}", maDC, maTK);
                return (false, _localizer["Common_Error"].Value);
            }
        }

        public async Task<(bool HopLe, string? ThongBaoLoi)> XoaDiaChiAsync(int maTK, int maDC)
        {
            var entity = await _context.DiaChiNhanHang.FirstOrDefaultAsync(d => d.MaDC == maDC && d.MaTK == maTK);
            if (entity == null)
            {
                return (false, _localizer["Msg_Address_NotFound"].Value);
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                bool wasDefault = entity.LaMacDinh;
                _context.DiaChiNhanHang.Remove(entity);
                await _context.SaveChangesAsync();

                // Nếu xóa địa chỉ mặc định mà còn địa chỉ khác -> địa chỉ mới nhất còn lại thành mặc định
                if (wasDefault)
                {
                    var newest = await _context.DiaChiNhanHang
                        .Where(d => d.MaTK == maTK)
                        .OrderByDescending(d => d.NgayTao)
                        .ThenByDescending(d => d.MaDC)
                        .FirstOrDefaultAsync();

                    if (newest != null)
                    {
                        newest.LaMacDinh = true;
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật CSDL khi xóa địa chỉ {MaDC} cho tài khoản {MaTK}", maDC, maTK);
                return (false, _localizer["Common_Error"].Value);
            }
        }

        public async Task<(bool HopLe, string? ThongBaoLoi)> DatMacDinhAsync(int maTK, int maDC)
        {
            // Kiểm tra địa chỉ tồn tại và thuộc maTK
            var exists = await _context.DiaChiNhanHang.AnyAsync(d => d.MaDC == maDC && d.MaTK == maTK);
            if (!exists)
            {
                return (false, _localizer["Msg_Address_NotFound"].Value);
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 1. Bỏ mặc định cũ
                await _context.DiaChiNhanHang
                    .Where(d => d.MaTK == maTK && d.LaMacDinh)
                    .ExecuteUpdateAsync(s => s.SetProperty(d => d.LaMacDinh, false));

                // 2. Đặt mặc định mới
                int rowsAffected = await _context.DiaChiNhanHang
                    .Where(d => d.MaDC == maDC && d.MaTK == maTK)
                    .ExecuteUpdateAsync(s => s.SetProperty(d => d.LaMacDinh, true));

                if (rowsAffected != 1)
                {
                    await transaction.RollbackAsync();
                    return (false, _localizer["Common_Error"].Value);
                }

                await transaction.CommitAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật CSDL khi đặt mặc định địa chỉ {MaDC} cho tài khoản {MaTK}", maDC, maTK);
                return (false, _localizer["Common_Error"].Value);
            }
        }

        public async Task<bool> ThemDiaChiTuDonHangAsync(int maTK, string tenNguoiNhan, string soDienThoai, string diaChi)
        {
            try
            {
                var ten = tenNguoiNhan?.Trim() ?? string.Empty;
                var sdt = soDienThoai?.Trim() ?? string.Empty;
                var dc = diaChi?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(dc))
                {
                    return false;
                }

                var currentCount = await _context.DiaChiNhanHang.CountAsync(d => d.MaTK == maTK);
                if (currentCount >= DiaChiConstant.ToiDaDiaChi)
                {
                    return false;
                }

                bool alreadyExists = await _context.DiaChiNhanHang.AnyAsync(d =>
                    d.MaTK == maTK &&
                    d.TenNguoiNhan == ten &&
                    d.SoDienThoai == sdt &&
                    d.DiaChi == dc);

                if (alreadyExists)
                {
                    return false;
                }

                bool laMacDinh = currentCount == 0;
                var entity = new DiaChiNhanHang
                {
                    MaTK = maTK,
                    TenNguoiNhan = ten,
                    SoDienThoai = sdt,
                    DiaChi = dc,
                    LaMacDinh = laMacDinh,
                    NgayTao = DateTime.Now
                };

                _context.DiaChiNhanHang.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể tự động lưu địa chỉ vào sổ sau khi đặt hàng cho tài khoản {MaTK}", maTK);
                return false;
            }
        }
    }
}
