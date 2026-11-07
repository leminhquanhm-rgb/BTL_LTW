using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Services
{
    public class DanhMucService : IDanhMucService
    {
        private readonly WebsiteBanMyPhamContext _context;

        public DanhMucService(WebsiteBanMyPhamContext context)
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

        public async Task<PaginatedResponseDto<DanhMucAdminDto>> LayDanhSachAdminAsync(
            string? keyword,
            bool? trangThai,
            int pageIndex,
            int pageSize)
        {
            int validPageIndex = pageIndex < 1 ? 1 : pageIndex;
            int validPageSize = Math.Clamp(pageSize, 1, 50);

            var query = _context.DanhMuc.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(d => d.TenDM.Contains(kw));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(d => d.TrangThai == trangThai.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)validPageSize);
            if (totalPages == 0) totalPages = 1;

            var items = await query
                .OrderBy(d => d.MaDM)
                .Skip((validPageIndex - 1) * validPageSize)
                .Take(validPageSize)
                .Select(d => new DanhMucAdminDto
                {
                    MaDM = d.MaDM,
                    TenDM = d.TenDM,
                    MoTa = d.MoTa,
                    TrangThai = d.TrangThai,
                    SoLuongSanPham = d.SanPham.Count()
                })
                .ToListAsync();

            return new PaginatedResponseDto<DanhMucAdminDto>
            {
                Items = items,
                PageIndex = validPageIndex,
                PageSize = validPageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<DanhMucAdminDto?> LayChiTietAdminAsync(int maDM)
        {
            return await _context.DanhMuc
                .AsNoTracking()
                .Where(d => d.MaDM == maDM)
                .Select(d => new DanhMucAdminDto
                {
                    MaDM = d.MaDM,
                    TenDM = d.TenDM,
                    MoTa = d.MoTa,
                    TrangThai = d.TrangThai,
                    SoLuongSanPham = d.SanPham.Count()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(LoaiLoiDanhMuc LoaiLoi, int MaDM, DanhMucAdminDto? Dto)> TaoAsync(
            string tenDM,
            string? moTa,
            bool trangThai)
        {
            var tenTrim = tenDM?.Trim() ?? string.Empty;
            var moTaTrim = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim();

            if (string.IsNullOrEmpty(tenTrim) || tenTrim.Length > 100 || (moTaTrim != null && moTaTrim.Length > 500))
            {
                return (LoaiLoiDanhMuc.DuLieuKhongHopLe, 0, null);
            }

            // Kiểm tra trùng tên trước (so sánh sau khi Trim)
            bool exists = await _context.DanhMuc.AnyAsync(d => d.TenDM == tenTrim);
            if (exists)
            {
                return (LoaiLoiDanhMuc.TrungTen, 0, null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var dm = new DanhMuc
                {
                    TenDM = tenTrim,
                    MoTa = moTaTrim,
                    TrangThai = trangThai
                };

                _context.DanhMuc.Add(dm);
                await _context.SaveChangesAsync();

                // EF Core scaffold HasDefaultValue(true) nên khi trangThai = false SQL tự gán 1.
                // Xử lý cập nhật đúng false trong transaction theo Điều chỉnh 1
                if (!trangThai)
                {
                    await _context.DanhMuc
                        .Where(d => d.MaDM == dm.MaDM)
                        .ExecuteUpdateAsync(s => s.SetProperty(d => d.TrangThai, false));
                    dm.TrangThai = false;
                }

                await transaction.CommitAsync();

                var dto = new DanhMucAdminDto
                {
                    MaDM = dm.MaDM,
                    TenDM = dm.TenDM,
                    MoTa = dm.MoTa,
                    TrangThai = dm.TrangThai,
                    SoLuongSanPham = 0
                };

                return (LoaiLoiDanhMuc.ThanhCong, dm.MaDM, dto);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                if (IsUniqueConstraintViolation(ex))
                {
                    return (LoaiLoiDanhMuc.TrungTen, 0, null);
                }
                throw;
            }
        }

        public async Task<(LoaiLoiDanhMuc LoaiLoi, DanhMucAdminDto? Dto)> CapNhatAsync(
            int maDM,
            string tenDM,
            string? moTa,
            bool trangThai)
        {
            var tenTrim = tenDM?.Trim() ?? string.Empty;
            var moTaTrim = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim();

            if (string.IsNullOrEmpty(tenTrim) || tenTrim.Length > 100 || (moTaTrim != null && moTaTrim.Length > 500))
            {
                return (LoaiLoiDanhMuc.DuLieuKhongHopLe, null);
            }

            var dm = await _context.DanhMuc.FirstOrDefaultAsync(d => d.MaDM == maDM);
            if (dm == null)
            {
                return (LoaiLoiDanhMuc.KhongTimThay, null);
            }

            // Kiểm tra trùng tên loại trừ chính bản ghi đang sửa (Điều chỉnh 2)
            bool exists = await _context.DanhMuc.AnyAsync(d => d.MaDM != maDM && d.TenDM == tenTrim);
            if (exists)
            {
                return (LoaiLoiDanhMuc.TrungTen, null);
            }

            try
            {
                dm.TenDM = tenTrim;
                dm.MoTa = moTaTrim;
                dm.TrangThai = trangThai;

                await _context.SaveChangesAsync();

                int soLuongSP = await _context.SanPham.CountAsync(s => s.MaDM == maDM);

                var dto = new DanhMucAdminDto
                {
                    MaDM = dm.MaDM,
                    TenDM = dm.TenDM,
                    MoTa = dm.MoTa,
                    TrangThai = dm.TrangThai,
                    SoLuongSanPham = soLuongSP
                };

                return (LoaiLoiDanhMuc.ThanhCong, dto);
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueConstraintViolation(ex))
                {
                    return (LoaiLoiDanhMuc.TrungTen, null);
                }
                throw;
            }
        }

        public async Task<(LoaiLoiDanhMuc LoaiLoi, int SoLuongSP)> XoaAsync(int maDM)
        {
            var dm = await _context.DanhMuc
                .Include(d => d.SanPham)
                .FirstOrDefaultAsync(d => d.MaDM == maDM);

            if (dm == null)
            {
                return (LoaiLoiDanhMuc.KhongTimThay, 0);
            }

            int soLuongSP = dm.SanPham.Count;
            if (soLuongSP > 0)
            {
                return (LoaiLoiDanhMuc.DangCoSanPhamThamChieu, soLuongSP);
            }

            try
            {
                _context.DanhMuc.Remove(dm);
                await _context.SaveChangesAsync();
                return (LoaiLoiDanhMuc.ThanhCong, 0);
            }
            catch (DbUpdateException ex)
            {
                // Bắt lỗi vi phạm khóa ngoại trường hợp có sản phẩm thêm đồng thời (Điều chỉnh 3)
                if (IsForeignKeyConstraintViolation(ex))
                {
                    int currentCount = await _context.SanPham.CountAsync(s => s.MaDM == maDM);
                    return (LoaiLoiDanhMuc.DangCoSanPhamThamChieu, currentCount > 0 ? currentCount : 1);
                }
                throw;
            }
        }

        public async Task<int> LaySoSanPhamCuaDanhMucAsync(int maDM)
        {
            return await _context.SanPham
                .CountAsync(s => s.MaDM == maDM && s.TrangThai && s.MaTHNavigation.TrangThai);
        }
    }
}

