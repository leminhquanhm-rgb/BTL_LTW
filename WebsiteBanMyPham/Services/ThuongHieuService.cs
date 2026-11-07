using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Services
{
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly WebsiteBanMyPhamContext _context;

        public ThuongHieuService(WebsiteBanMyPhamContext context)
        {
            _context = context;
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;
            }
            return false;
        }

        private static bool IsForeignKeyConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number == 547;
            }
            return false;
        }

        public async Task<PaginatedResponseDto<ThuongHieuAdminDto>> LayDanhSachAdminAsync(
            string? keyword,
            bool? trangThai,
            int pageIndex,
            int pageSize)
        {
            int validPageIndex = pageIndex < 1 ? 1 : pageIndex;
            int validPageSize = Math.Clamp(pageSize, 1, 50);

            var query = _context.ThuongHieu.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(t => t.TenTH.Contains(kw));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)validPageSize);
            if (totalPages == 0) totalPages = 1;

            var items = await query
                .OrderBy(t => t.MaTH)
                .Skip((validPageIndex - 1) * validPageSize)
                .Take(validPageSize)
                .Select(t => new ThuongHieuAdminDto
                {
                    MaTH = t.MaTH,
                    TenTH = t.TenTH,
                    Logo = t.Logo,
                    MoTa = t.MoTa,
                    TrangThai = t.TrangThai,
                    SoLuongSanPham = t.SanPham.Count()
                })
                .ToListAsync();

            return new PaginatedResponseDto<ThuongHieuAdminDto>
            {
                Items = items,
                PageIndex = validPageIndex,
                PageSize = validPageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<ThuongHieuAdminDto?> LayChiTietAdminAsync(int maTH)
        {
            return await _context.ThuongHieu
                .AsNoTracking()
                .Where(t => t.MaTH == maTH)
                .Select(t => new ThuongHieuAdminDto
                {
                    MaTH = t.MaTH,
                    TenTH = t.TenTH,
                    Logo = t.Logo,
                    MoTa = t.MoTa,
                    TrangThai = t.TrangThai,
                    SoLuongSanPham = t.SanPham.Count()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(LoaiLoiThuongHieu LoaiLoi, int MaTH, ThuongHieuAdminDto? Dto)> TaoAsync(
            string tenTH,
            string? moTa,
            bool trangThai,
            string? logo = null)
        {
            var tenTrim = tenTH?.Trim() ?? string.Empty;
            var moTaTrim = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim();

            if (string.IsNullOrEmpty(tenTrim) || tenTrim.Length > 100 || (moTaTrim != null && moTaTrim.Length > 500))
            {
                return (LoaiLoiThuongHieu.DuLieuKhongHopLe, 0, null);
            }

            bool exists = await _context.ThuongHieu.AnyAsync(t => t.TenTH == tenTrim);
            if (exists)
            {
                return (LoaiLoiThuongHieu.TrungTen, 0, null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var th = new ThuongHieu
                {
                    TenTH = tenTrim,
                    MoTa = moTaTrim,
                    Logo = string.IsNullOrWhiteSpace(logo) ? null : logo.Trim(),
                    TrangThai = trangThai
                };

                _context.ThuongHieu.Add(th);
                await _context.SaveChangesAsync();

                // EF Core scaffold HasDefaultValue(true): nếu trangThai = false thì cập nhật lại trong transaction
                if (!trangThai)
                {
                    await _context.ThuongHieu
                        .Where(t => t.MaTH == th.MaTH)
                        .ExecuteUpdateAsync(s => s.SetProperty(t => t.TrangThai, false));
                    th.TrangThai = false;
                }

                await transaction.CommitAsync();

                var dto = new ThuongHieuAdminDto
                {
                    MaTH = th.MaTH,
                    TenTH = th.TenTH,
                    Logo = th.Logo,
                    MoTa = th.MoTa,
                    TrangThai = th.TrangThai,
                    SoLuongSanPham = 0
                };

                return (LoaiLoiThuongHieu.ThanhCong, th.MaTH, dto);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                if (IsUniqueConstraintViolation(ex))
                {
                    return (LoaiLoiThuongHieu.TrungTen, 0, null);
                }
                throw;
            }
        }

        public async Task<(LoaiLoiThuongHieu LoaiLoi, ThuongHieuAdminDto? Dto)> CapNhatAsync(
            int maTH,
            string tenTH,
            string? moTa,
            bool trangThai,
            string? logo = null,
            bool capNhatLogo = false)
        {
            var tenTrim = tenTH?.Trim() ?? string.Empty;
            var moTaTrim = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim();

            if (string.IsNullOrEmpty(tenTrim) || tenTrim.Length > 100 || (moTaTrim != null && moTaTrim.Length > 500))
            {
                return (LoaiLoiThuongHieu.DuLieuKhongHopLe, null);
            }

            var th = await _context.ThuongHieu.FirstOrDefaultAsync(t => t.MaTH == maTH);
            if (th == null)
            {
                return (LoaiLoiThuongHieu.KhongTimThay, null);
            }

            // Kiểm tra trùng tên loại trừ chính bản ghi đang sửa (Điều chỉnh 2)
            bool exists = await _context.ThuongHieu.AnyAsync(t => t.MaTH != maTH && t.TenTH == tenTrim);
            if (exists)
            {
                return (LoaiLoiThuongHieu.TrungTen, null);
            }

            try
            {
                th.TenTH = tenTrim;
                th.MoTa = moTaTrim;
                if (capNhatLogo)
                {
                    th.Logo = string.IsNullOrWhiteSpace(logo) ? null : logo.Trim();
                }
                th.TrangThai = trangThai;

                await _context.SaveChangesAsync();

                int soLuongSP = await _context.SanPham.CountAsync(s => s.MaTH == maTH);

                var dto = new ThuongHieuAdminDto
                {
                    MaTH = th.MaTH,
                    TenTH = th.TenTH,
                    Logo = th.Logo,
                    MoTa = th.MoTa,
                    TrangThai = th.TrangThai,
                    SoLuongSanPham = soLuongSP
                };

                return (LoaiLoiThuongHieu.ThanhCong, dto);
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueConstraintViolation(ex))
                {
                    return (LoaiLoiThuongHieu.TrungTen, null);
                }
                throw;
            }
        }

        public async Task<(LoaiLoiThuongHieu LoaiLoi, int SoLuongSP)> XoaAsync(int maTH)
        {
            var th = await _context.ThuongHieu
                .Include(t => t.SanPham)
                .FirstOrDefaultAsync(t => t.MaTH == maTH);

            if (th == null)
            {
                return (LoaiLoiThuongHieu.KhongTimThay, 0);
            }

            int soLuongSP = th.SanPham.Count;
            if (soLuongSP > 0)
            {
                return (LoaiLoiThuongHieu.DangCoSanPhamThamChieu, soLuongSP);
            }

            try
            {
                _context.ThuongHieu.Remove(th);
                await _context.SaveChangesAsync();
                return (LoaiLoiThuongHieu.ThanhCong, 0);
            }
            catch (DbUpdateException ex)
            {
                // Bắt lỗi vi phạm khóa ngoại trường hợp có sản phẩm thêm đồng thời (Điều chỉnh 3)
                if (IsForeignKeyConstraintViolation(ex))
                {
                    int currentCount = await _context.SanPham.CountAsync(s => s.MaTH == maTH);
                    return (LoaiLoiThuongHieu.DangCoSanPhamThamChieu, currentCount > 0 ? currentCount : 1);
                }
                throw;
            }
        }

        public async Task<int> LaySoSanPhamCuaThuongHieuAsync(int maTH)
        {
            return await _context.SanPham
                .CountAsync(s => s.MaTH == maTH && s.TrangThai && s.MaDMNavigation.TrangThai);
        }
    }
}

