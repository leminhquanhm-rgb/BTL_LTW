using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class SanPhamAdminService : ISanPhamAdminService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHtmlSanitizerService _htmlSanitizerService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<SanPhamAdminService> _logger;

        public SanPhamAdminService(
            WebsiteBanMyPhamContext context,
            IFileUploadService fileUploadService,
            IHtmlSanitizerService htmlSanitizerService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<SanPhamAdminService> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _htmlSanitizerService = htmlSanitizerService;
            _localizer = localizer;
            _logger = logger;
        }

        private static string XacDinhTrangThaiHienThiKey(bool spTrangThai, bool dmTrangThai, bool thTrangThai)
        {
            if (!spTrangThai) return "Admin_Product_Status_Stopped";
            if (!dmTrangThai) return "Admin_Product_Status_HiddenCategory";
            if (!thTrangThai) return "Admin_Product_Status_HiddenBrand";
            return "Admin_Product_Status_OnSale";
        }

        public async Task<PaginatedList<SanPhamAdminListItemViewModel>> LayDanhSachAdminAsync(SanPhamAdminFilterViewModel filter)
        {
            int validTrang = filter.Trang < 1 ? 1 : filter.Trang;
            int validPageSize = Math.Clamp(filter.PageSize, 1, 50);

            var query = _context.SanPham
                .AsNoTracking()
                .Include(s => s.MaDMNavigation)
                .Include(s => s.MaTHNavigation)
                .Include(s => s.AnhSanPham)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(s => s.TenSP.Contains(kw) || s.MaTHNavigation.TenTH.Contains(kw));
            }

            if (filter.MaDM.HasValue && filter.MaDM.Value > 0)
            {
                query = query.Where(s => s.MaDM == filter.MaDM.Value);
            }

            if (filter.MaTH.HasValue && filter.MaTH.Value > 0)
            {
                query = query.Where(s => s.MaTH == filter.MaTH.Value);
            }

            if (filter.TrangThai.HasValue)
            {
                query = query.Where(s => s.TrangThai == filter.TrangThai.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.TinhTrangTon))
            {
                if (filter.TinhTrangTon == "sapHet")
                {
                    query = query.Where(s => s.SoLuongTon > 0 && s.SoLuongTon <= s.NguongCanhBao);
                }
                else if (filter.TinhTrangTon == "hetHang")
                {
                    query = query.Where(s => s.SoLuongTon <= 0);
                }
            }

            int totalItems = await query.CountAsync();

            var rawItems = await query
                .OrderByDescending(s => s.MaSP)
                .Skip((validTrang - 1) * validPageSize)
                .Take(validPageSize)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).ThenBy(a => a.MaAnh).Select(a => a.DuongDan).FirstOrDefault(),
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.NguongCanhBao,
                    s.TrangThai,
                    TrangThaiDanhMuc = s.MaDMNavigation.TrangThai,
                    TrangThaiThuongHieu = s.MaTHNavigation.TrangThai,
                    SoLuongAnh = s.AnhSanPham.Count
                })
                .ToListAsync();

            var items = rawItems.Select(s => new SanPhamAdminListItemViewModel
            {
                MaSP = s.MaSP,
                TenSP = s.TenSP,
                AnhChinh = s.AnhChinh ?? "/images/placeholder.svg",
                MaDM = s.MaDM,
                TenDanhMuc = s.TenDanhMuc,
                MaTH = s.MaTH,
                TenThuongHieu = s.TenThuongHieu,
                Gia = s.Gia,
                GiaGoc = s.GiaGoc,
                SoLuongTon = s.SoLuongTon,
                NguongCanhBao = s.NguongCanhBao,
                TrangThai = s.TrangThai,
                TrangThaiDanhMuc = s.TrangThaiDanhMuc,
                TrangThaiThuongHieu = s.TrangThaiThuongHieu,
                IsDangBan = s.TrangThai && s.TrangThaiDanhMuc && s.TrangThaiThuongHieu,
                TrangThaiHienThiKey = XacDinhTrangThaiHienThiKey(s.TrangThai, s.TrangThaiDanhMuc, s.TrangThaiThuongHieu),
                SoLuongAnh = s.SoLuongAnh
            }).ToList();

            return new PaginatedList<SanPhamAdminListItemViewModel>(items, totalItems, validTrang, validPageSize);
        }

        public async Task<SanPhamAdminDetailViewModel?> LayChiTietAdminAsync(int maSP)
        {
            var s = await _context.SanPham
                .AsNoTracking()
                .Include(p => p.MaDMNavigation)
                .Include(p => p.MaTHNavigation)
                .Include(p => p.AnhSanPham)
                .FirstOrDefaultAsync(p => p.MaSP == maSP);

            if (s == null) return null;

            return new SanPhamAdminDetailViewModel
            {
                MaSP = s.MaSP,
                TenSP = s.TenSP,
                MoTa = s.MoTa,
                Gia = s.Gia,
                GiaGoc = s.GiaGoc,
                SoLuongTon = s.SoLuongTon,
                NguongCanhBao = s.NguongCanhBao,
                TrangThai = s.TrangThai,
                MaDM = s.MaDM,
                TenDanhMuc = s.MaDMNavigation.TenDM,
                TrangThaiDanhMuc = s.MaDMNavigation.TrangThai,
                MaTH = s.MaTH,
                TenThuongHieu = s.MaTHNavigation.TenTH,
                TrangThaiThuongHieu = s.MaTHNavigation.TrangThai,
                IsDangBan = s.TrangThai && s.MaDMNavigation.TrangThai && s.MaTHNavigation.TrangThai,
                NgayTao = s.NgayTao,
                DanhSachAnh = s.AnhSanPham
                    .OrderByDescending(a => a.LaAnhChinh)
                    .ThenBy(a => a.MaAnh)
                    .Select(a => new AnhSanPhamItemViewModel
                    {
                        MaAnh = a.MaAnh,
                        MaSP = a.MaSP,
                        DuongDan = a.DuongDan,
                        LaAnhChinh = a.LaAnhChinh
                    }).ToList()
            };
        }

        public async Task<SanPhamCreateFormDropdownsViewModel> LayDuLieuFormThemAsync()
        {
            var danhMucs = await _context.DanhMuc
                .AsNoTracking()
                .Where(d => d.TrangThai)
                .OrderBy(d => d.TenDM)
                .Select(d => new SelectListItem { Value = d.MaDM.ToString(), Text = d.TenDM })
                .ToListAsync();

            var thuongHieus = await _context.ThuongHieu
                .AsNoTracking()
                .Where(t => t.TrangThai)
                .OrderBy(t => t.TenTH)
                .Select(t => new SelectListItem { Value = t.MaTH.ToString(), Text = t.TenTH })
                .ToListAsync();

            return new SanPhamCreateFormDropdownsViewModel
            {
                DanhMucs = danhMucs,
                ThuongHieus = thuongHieus
            };
        }

        public async Task<SanPhamEditFormDropdownsViewModel> LayDuLieuFormSuaAsync(int maSP)
        {
            var currentProduct = await _context.SanPham
                .AsNoTracking()
                .Where(s => s.MaSP == maSP)
                .Select(s => new { s.MaDM, s.MaTH })
                .FirstOrDefaultAsync();

            int currentMaDM = currentProduct?.MaDM ?? 0;
            int currentMaTH = currentProduct?.MaTH ?? 0;

            var danhMucsRaw = await _context.DanhMuc
                .AsNoTracking()
                .Where(d => d.TrangThai || d.MaDM == currentMaDM)
                .OrderBy(d => d.TenDM)
                .ToListAsync();

            var thuongHieusRaw = await _context.ThuongHieu
                .AsNoTracking()
                .Where(t => t.TrangThai || t.MaTH == currentMaTH)
                .OrderBy(t => t.TenTH)
                .ToListAsync();

            var inactiveSuffix = $" ({_localizer["Admin_Filter_Inactive"].Value.ToLower()})";

            var danhMucs = danhMucsRaw.Select(d => new SelectListItem
            {
                Value = d.MaDM.ToString(),
                Text = d.TrangThai ? d.TenDM : $"{d.TenDM}{inactiveSuffix}"
            }).ToList();

            var thuongHieus = thuongHieusRaw.Select(t => new SelectListItem
            {
                Value = t.MaTH.ToString(),
                Text = t.TrangThai ? t.TenTH : $"{t.TenTH}{inactiveSuffix}"
            }).ToList();

            return new SanPhamEditFormDropdownsViewModel
            {
                DanhMucs = danhMucs,
                ThuongHieus = thuongHieus
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int MaSP)> TaoSanPhamAsync(
            SanPhamCreateViewModel model, List<IFormFile>? imageFiles)
        {
            var tenTrim = model.TenSP?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenTrim) || tenTrim.Length > 200)
            {
                return (false, _localizer["Validation_Required"].Value, 0);
            }

            if (model.Gia < 0)
            {
                return (false, _localizer["Validation_Range"].Value, 0);
            }

            if (model.GiaGoc.HasValue && model.GiaGoc.Value < model.Gia)
            {
                return (false, _localizer["Admin_Product_Validation_OriginalPrice"].Value, 0);
            }

            if (model.SoLuongTon < 0 || model.NguongCanhBao < 0)
            {
                return (false, _localizer["Validation_Range"].Value, 0);
            }

            // Kiểm tra danh mục và thương hiệu tồn tại và đang hoạt động
            var dm = await _context.DanhMuc.AsNoTracking().FirstOrDefaultAsync(d => d.MaDM == model.MaDM);
            if (dm == null || !dm.TrangThai)
            {
                return (false, _localizer["Admin_Product_Validation_CategoryInactive"].Value, 0);
            }

            var th = await _context.ThuongHieu.AsNoTracking().FirstOrDefaultAsync(t => t.MaTH == model.MaTH);
            if (th == null || !th.TrangThai)
            {
                return (false, _localizer["Admin_Product_Validation_BrandInactive"].Value, 0);
            }

            // Làm sạch rich text HTML
            var sanitizedMoTa = _htmlSanitizerService.Sanitize(model.MoTa);
            if (!_htmlSanitizerService.KiemTraDoDaiHopLe(sanitizedMoTa))
            {
                return (false, _localizer["Admin_Product_Validation_DescTooLong"].Value, 0);
            }

            // Điều chỉnh 4: Kiểm tra số lượng và tính hợp lệ của TẤT CẢ ảnh trước khi ghi database
            var uploadedPaths = new List<string>();
            var validFiles = imageFiles?.Where(f => f != null && f.Length > 0).ToList() ?? new List<IFormFile>();

            if (validFiles.Count > 8)
            {
                return (false, _localizer["Admin_Product_Images_MaxExceeded"].Value, 0);
            }

            foreach (var file in validFiles)
            {
                var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(file, UploadFolderConstant.Products);
                if (!uploadSuccess)
                {
                    // Dọn dẹp toàn bộ file vừa tải lên của batch này nếu có 1 file lỗi
                    foreach (var p in uploadedPaths)
                    {
                        _fileUploadService.DeleteFile(p, UploadFolderConstant.Products);
                    }
                    var err = string.Format(_localizer["Admin_Product_Validation_InvalidImage"].Value, file.FileName, uploadError ?? _localizer["Upload_Error_System"].Value);
                    return (false, err, 0);
                }
                uploadedPaths.Add(filePath!);
            }

            // Điều chỉnh 3: Bắt lỗi, rollback, xóa file vừa tải, ghi log, không throw ra ngoài
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sp = new SanPham
                {
                    TenSP = tenTrim,
                    MoTa = sanitizedMoTa,
                    Gia = model.Gia,
                    GiaGoc = model.GiaGoc,
                    SoLuongTon = model.SoLuongTon,
                    NguongCanhBao = model.NguongCanhBao,
                    TrangThai = model.TrangThai,
                    MaDM = model.MaDM,
                    MaTH = model.MaTH,
                    NgayTao = DateTime.Now
                };

                _context.SanPham.Add(sp);
                await _context.SaveChangesAsync();

                // Xử lý EF Core HasDefaultValue(true) và HasDefaultValue(5)
                if (!model.TrangThai || model.NguongCanhBao == 0)
                {
                    await _context.SanPham
                        .Where(s => s.MaSP == sp.MaSP)
                        .ExecuteUpdateAsync(setter => setter
                            .SetProperty(p => p.TrangThai, model.TrangThai)
                            .SetProperty(p => p.NguongCanhBao, model.NguongCanhBao));

                    sp.TrangThai = model.TrangThai;
                    sp.NguongCanhBao = model.NguongCanhBao;
                }

                // Lưu danh sách ảnh: ảnh đầu tiên tự là ảnh chính
                if (uploadedPaths.Count > 0)
                {
                    for (int i = 0; i < uploadedPaths.Count; i++)
                    {
                        var anh = new AnhSanPham
                        {
                            MaSP = sp.MaSP,
                            DuongDan = uploadedPaths[i],
                            LaAnhChinh = (i == 0)
                        };
                        _context.AnhSanPham.Add(anh);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return (true, null, sp.MaSP);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi tạo sản phẩm mới {TenSP}", tenTrim);

                foreach (var p in uploadedPaths)
                {
                    _fileUploadService.DeleteFile(p, UploadFolderConstant.Products);
                }

                return (false, _localizer["Upload_Error_System"].Value, 0);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatSanPhamAsync(int maSP, SanPhamEditViewModel model)
        {
            var tenTrim = model.TenSP?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenTrim) || tenTrim.Length > 200)
            {
                return (false, _localizer["Validation_Required"].Value);
            }

            if (model.Gia < 0)
            {
                return (false, _localizer["Validation_Range"].Value);
            }

            if (model.GiaGoc.HasValue && model.GiaGoc.Value < model.Gia)
            {
                return (false, _localizer["Admin_Product_Validation_OriginalPrice"].Value);
            }

            if (model.NguongCanhBao < 0)
            {
                return (false, _localizer["Validation_Range"].Value);
            }

            var dmExists = await _context.DanhMuc.AnyAsync(d => d.MaDM == model.MaDM);
            if (!dmExists)
            {
                return (false, _localizer["Admin_Product_Validation_CategoryInactive"].Value);
            }

            var thExists = await _context.ThuongHieu.AnyAsync(t => t.MaTH == model.MaTH);
            if (!thExists)
            {
                return (false, _localizer["Admin_Product_Validation_BrandInactive"].Value);
            }

            var sanitizedMoTa = _htmlSanitizerService.Sanitize(model.MoTa);
            if (!_htmlSanitizerService.KiemTraDoDaiHopLe(sanitizedMoTa))
            {
                return (false, _localizer["Admin_Product_Validation_DescTooLong"].Value);
            }

            // Điều chỉnh 2: Đọc entity SanPham đang theo dõi từ database rồi CHỈ gán các trường được phép sửa.
            // TUYỆT ĐỐI không dùng _context.Update(...) với đối tượng dựng từ form, không gán SoLuongTon, NgayTao.
            var sp = await _context.SanPham.FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            try
            {
                sp.TenSP = tenTrim;
                sp.MoTa = sanitizedMoTa;
                sp.Gia = model.Gia;
                sp.GiaGoc = model.GiaGoc;
                sp.NguongCanhBao = model.NguongCanhBao;
                sp.TrangThai = model.TrangThai;
                sp.MaDM = model.MaDM;
                sp.MaTH = model.MaTH;

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi cập nhật sản phẩm MaSP {MaSP}", maSP);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> ChuyenTrangThaiKinhDoanhAsync(int maSP)
        {
            var sp = await _context.SanPham.FirstOrDefaultAsync(s => s.MaSP == maSP);
            if (sp == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            try
            {
                sp.TrangThai = !sp.TrangThai;
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chuyển trạng thái kinh doanh MaSP {MaSP}", maSP);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage, AnhSanPhamItemViewModel? AnhMoi)> ThemAnhAsync(int maSP, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, _localizer["Upload_Error_NoFile"].Value, null);
            }

            var spExists = await _context.SanPham.AnyAsync(s => s.MaSP == maSP);
            if (!spExists)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value, null);
            }

            int currentCount = await _context.AnhSanPham.CountAsync(a => a.MaSP == maSP);
            if (currentCount >= 8)
            {
                return (false, _localizer["Admin_Product_Images_MaxExceeded"].Value, null);
            }

            var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(file, UploadFolderConstant.Products);
            if (!uploadSuccess || filePath == null)
            {
                return (false, uploadError ?? _localizer["Upload_Error_System"].Value, null);
            }

            try
            {
                var anh = new AnhSanPham
                {
                    MaSP = maSP,
                    DuongDan = filePath,
                    LaAnhChinh = (currentCount == 0)
                };

                _context.AnhSanPham.Add(anh);
                await _context.SaveChangesAsync();

                var dto = new AnhSanPhamItemViewModel
                {
                    MaAnh = anh.MaAnh,
                    MaSP = anh.MaSP,
                    DuongDan = anh.DuongDan,
                    LaAnhChinh = anh.LaAnhChinh
                };

                return (true, null, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu ảnh sản phẩm MaSP {MaSP}", maSP);
                _fileUploadService.DeleteFile(filePath, UploadFolderConstant.Products);
                return (false, _localizer["Upload_Error_System"].Value, null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DatAnhChinhAsync(int maSP, int maAnh)
        {
            var anh = await _context.AnhSanPham.FirstOrDefaultAsync(a => a.MaAnh == maAnh && a.MaSP == maSP);
            if (anh == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AnhSanPham
                    .Where(a => a.MaSP == maSP)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.LaAnhChinh, false));

                await _context.AnhSanPham
                    .Where(a => a.MaAnh == maAnh && a.MaSP == maSP)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.LaAnhChinh, true));

                await transaction.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi đặt ảnh chính MaAnh {MaAnh} cho MaSP {MaSP}", maAnh, maSP);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaAnhAsync(int maSP, int maAnh)
        {
            var anh = await _context.AnhSanPham.FirstOrDefaultAsync(a => a.MaAnh == maAnh && a.MaSP == maSP);
            if (anh == null)
            {
                return (false, _localizer["Api_Error_NotFoundTitle"].Value);
            }

            string duongDan = anh.DuongDan;
            bool laAnhChinh = anh.LaAnhChinh;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AnhSanPham.Remove(anh);
                await _context.SaveChangesAsync();

                // Nếu xóa ảnh chính, tự động chọn ảnh có MaAnh nhỏ nhất còn lại làm ảnh chính
                if (laAnhChinh)
                {
                    var nextMain = await _context.AnhSanPham
                        .Where(a => a.MaSP == maSP && a.MaAnh != maAnh)
                        .OrderBy(a => a.MaAnh)
                        .FirstOrDefaultAsync();

                    if (nextMain != null)
                    {
                        nextMain.LaAnhChinh = true;
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                // Xóa file sau khi DB thành công
                _fileUploadService.DeleteFile(duongDan, UploadFolderConstant.Products);
                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi xóa ảnh MaAnh {MaAnh} của MaSP {MaSP}", maAnh, maSP);
                return (false, _localizer["Upload_Error_System"].Value);
            }
        }

        public async Task<PaginatedList<TonKhoListItemViewModel>> LayDanhSachTonKhoAsync(TonKhoFilterViewModel filter)
        {
            int validTrang = filter.Trang < 1 ? 1 : filter.Trang;
            int validPageSize = Math.Clamp(filter.PageSize, 1, 50);

            var query = _context.SanPham
                .AsNoTracking()
                .Include(s => s.MaDMNavigation)
                .Include(s => s.MaTHNavigation)
                .Include(s => s.AnhSanPham)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(s => s.TenSP.Contains(kw) || s.MaTHNavigation.TenTH.Contains(kw));
            }

            if (filter.MaDM.HasValue && filter.MaDM.Value > 0)
            {
                query = query.Where(s => s.MaDM == filter.MaDM.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.TinhTrangTon))
            {
                if (filter.TinhTrangTon == "sapHet")
                {
                    query = query.Where(s => s.SoLuongTon > 0 && s.SoLuongTon <= s.NguongCanhBao);
                }
                else if (filter.TinhTrangTon == "hetHang")
                {
                    query = query.Where(s => s.SoLuongTon <= 0);
                }
            }

            int totalItems = await query.CountAsync();

            var rawItems = await query
                .OrderBy(s => s.SoLuongTon) // Ưu tiên hàng sắp hết/hết hàng lên trước
                .ThenBy(s => s.MaSP)
                .Skip((validTrang - 1) * validPageSize)
                .Take(validPageSize)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).ThenBy(a => a.MaAnh).Select(a => a.DuongDan).FirstOrDefault(),
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    s.SoLuongTon,
                    s.NguongCanhBao,
                    s.TrangThai,
                    TrangThaiDanhMuc = s.MaDMNavigation.TrangThai,
                    TrangThaiThuongHieu = s.MaTHNavigation.TrangThai
                })
                .ToListAsync();

            var items = rawItems.Select(s => new TonKhoListItemViewModel
            {
                MaSP = s.MaSP,
                TenSP = s.TenSP,
                AnhChinh = s.AnhChinh ?? "/images/placeholder.svg",
                MaDM = s.MaDM,
                TenDanhMuc = s.TenDanhMuc,
                MaTH = s.MaTH,
                TenThuongHieu = s.TenThuongHieu,
                SoLuongTon = s.SoLuongTon,
                NguongCanhBao = s.NguongCanhBao,
                IsDangBan = s.TrangThai && s.TrangThaiDanhMuc && s.TrangThaiThuongHieu,
                TrangThaiHienThiKey = XacDinhTrangThaiHienThiKey(s.TrangThai, s.TrangThaiDanhMuc, s.TrangThaiThuongHieu)
            }).ToList();

            return new PaginatedList<TonKhoListItemViewModel>(items, totalItems, validTrang, validPageSize);
        }

        public async Task<(bool Success, string? ErrorMessage, int SoLuongTonMoi)> DieuChinhTonKhoAsync(int maSP, int delta)
        {
            // Điều chỉnh 5: delta khác 0 và |delta| <= 100.000
            if (delta == 0 || Math.Abs(delta) > 100000)
            {
                return (false, _localizer["Admin_Inventory_Adjust_InvalidDelta"].Value, 0);
            }

            // Điều chỉnh 5: Đúng một câu ExecuteUpdateAsync có điều kiện
            var affected = await _context.SanPham
                .Where(s => s.MaSP == maSP && s.SoLuongTon + delta >= 0)
                .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.SoLuongTon, p => p.SoLuongTon + delta));

            if (affected == 0)
            {
                bool exists = await _context.SanPham.AnyAsync(s => s.MaSP == maSP);
                if (!exists)
                {
                    return (false, _localizer["Admin_Inventory_Adjust_NotFound"].Value, 0);
                }
                return (false, _localizer["Admin_Inventory_Adjust_Failed"].Value, 0);
            }

            // Đọc lại tồn mới sau cập nhật
            int newStock = await _context.SanPham
                .Where(s => s.MaSP == maSP)
                .Select(s => s.SoLuongTon)
                .FirstOrDefaultAsync();

            return (true, null, newStock);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatNguongCanhBaoAsync(int maSP, int nguongCanhBao)
        {
            if (nguongCanhBao < 0 || nguongCanhBao > 100000)
            {
                return (false, _localizer["Validation_Range"].Value);
            }

            var affected = await _context.SanPham
                .Where(s => s.MaSP == maSP)
                .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.NguongCanhBao, nguongCanhBao));

            if (affected == 0)
            {
                return (false, _localizer["Admin_Inventory_Adjust_NotFound"].Value);
            }

            return (true, null);
        }
    }
}

