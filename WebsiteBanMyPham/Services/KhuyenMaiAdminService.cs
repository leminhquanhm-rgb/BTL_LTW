using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public class KhuyenMaiAdminService : IKhuyenMaiAdminService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhuyenMaiAdminService(
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<PaginatedList<KhuyenMaiAdminListItemViewModel>> LayDanhSachAsync(KhuyenMaiAdminFilterViewModel filter)
        {
            var query = _context.KhuyenMai.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(k => k.Code.Contains(kw) || k.TenKM.Contains(kw));
            }

            if (filter.TrangThai.HasValue)
            {
                query = query.Where(k => k.TrangThai == filter.TrangThai.Value);
            }

            // Sắp xếp: Mã đang hoạt động / thời gian mới nhất lên trước
            query = query.OrderByDescending(k => k.NgayKetThuc).ThenByDescending(k => k.NgayBatDau);

            int totalItems = await query.CountAsync();
            int validPage = Math.Max(1, filter.Trang);
            int validSize = Math.Clamp(filter.PageSize, 1, 50);

            var items = await query
                .Skip((validPage - 1) * validSize)
                .Take(validSize)
                .Select(k => new
                {
                    k.MaKM,
                    k.Code,
                    k.TenKM,
                    k.LoaiGiam,
                    k.GiaTriGiam,
                    k.GiamToiDa,
                    k.DonToiThieu,
                    k.NgayBatDau,
                    k.NgayKetThuc,
                    k.SoLuotToiDa,
                    k.SoLuotDaDung,
                    k.TrangThai,
                    CoDonHang = _context.DonHang.Any(d => d.MaKM == k.MaKM)
                })
                .ToListAsync();

            var now = DateTime.Now;
            var mappedItems = items.Select(k =>
            {
                string tinhToanStatus;
                if (!k.TrangThai)
                {
                    tinhToanStatus = "TamTat";
                }
                else if (now > k.NgayKetThuc)
                {
                    tinhToanStatus = "HetHan";
                }
                else if (k.SoLuotToiDa.HasValue && k.SoLuotDaDung >= k.SoLuotToiDa.Value)
                {
                    tinhToanStatus = "HetLuot";
                }
                else if (now < k.NgayBatDau)
                {
                    tinhToanStatus = "SapDienRa";
                }
                else
                {
                    tinhToanStatus = "DangChay";
                }

                return new KhuyenMaiAdminListItemViewModel
                {
                    MaKM = k.MaKM,
                    Code = k.Code,
                    TenKM = k.TenKM,
                    LoaiGiam = k.LoaiGiam,
                    GiaTriGiam = k.GiaTriGiam,
                    GiamToiDa = k.GiamToiDa,
                    DonToiThieu = k.DonToiThieu,
                    NgayBatDau = k.NgayBatDau,
                    NgayKetThuc = k.NgayKetThuc,
                    SoLuotToiDa = k.SoLuotToiDa,
                    SoLuotDaDung = k.SoLuotDaDung,
                    TrangThai = k.TrangThai,
                    TrangThaiTinhToan = tinhToanStatus,
                    CoDonHangThamChieu = k.CoDonHang
                };
            }).ToList();

            return new PaginatedList<KhuyenMaiAdminListItemViewModel>(mappedItems, totalItems, validPage, validSize);
        }

        public async Task<KhuyenMaiAdminFormViewModel?> LayDeSuaAsync(int maKM)
        {
            var km = await _context.KhuyenMai.AsNoTracking().FirstOrDefaultAsync(k => k.MaKM == maKM);
            if (km == null) return null;

            return new KhuyenMaiAdminFormViewModel
            {
                MaKM = km.MaKM,
                Code = km.Code,
                TenKM = km.TenKM,
                LoaiGiam = km.LoaiGiam,
                GiaTriGiam = km.GiaTriGiam,
                GiamToiDa = km.GiamToiDa,
                DonToiThieu = km.DonToiThieu,
                NgayBatDau = km.NgayBatDau.Date,
                NgayKetThuc = km.NgayKetThuc.Date,
                SoLuotToiDa = km.SoLuotToiDa,
                SoLuotDaDung = km.SoLuotDaDung,
                TrangThai = km.TrangThai,
                CoTheDoiCode = km.SoLuotDaDung == 0
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? MaKM)> ThemKhuyenMaiAsync(KhuyenMaiAdminFormViewModel model)
        {
            var code = model.Code?.Trim().ToUpperInvariant() ?? string.Empty;
            if (string.IsNullOrEmpty(code) || !Regex.IsMatch(code, @"^[A-Za-z0-9]+$"))
            {
                return (false, _localizer["Admin_Promo_CodeHint"].Value, null);
            }

            if (await _context.KhuyenMai.AnyAsync(k => k.Code == code))
            {
                return (false, string.Format(_localizer["Admin_Promo_Error_DuplicateCode"].Value, code), null);
            }

            var tenKM = model.TenKM?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(tenKM))
            {
                return (false, _localizer["Validation_Required"].Value, null);
            }

            if (model.LoaiGiam == LoaiGiamGiaConstant.PhanTram)
            {
                if (model.GiaTriGiam <= 0 || model.GiaTriGiam > 100)
                {
                    return (false, _localizer["Admin_Promo_Error_PercentValue"].Value, null);
                }
            }
            else if (model.LoaiGiam == LoaiGiamGiaConstant.SoTien)
            {
                if (model.GiaTriGiam <= 0)
                {
                    return (false, _localizer["Admin_Promo_Error_AmountValue"].Value, null);
                }
            }
            else
            {
                return (false, _localizer["Validation_Required"].Value, null);
            }

            if (model.NgayKetThuc.Date < model.NgayBatDau.Date)
            {
                return (false, _localizer["Validation_DateRangeInvalid"].Value, null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var km = new KhuyenMai
                {
                    Code = code,
                    TenKM = tenKM,
                    LoaiGiam = model.LoaiGiam,
                    GiaTriGiam = model.GiaTriGiam,
                    GiamToiDa = model.LoaiGiam == LoaiGiamGiaConstant.PhanTram ? model.GiamToiDa : null,
                    DonToiThieu = Math.Max(0, model.DonToiThieu),
                    NgayBatDau = model.NgayBatDau.Date,
                    NgayKetThuc = model.NgayKetThuc.Date.AddDays(1).AddTicks(-1),
                    SoLuotToiDa = model.SoLuotToiDa,
                    SoLuotDaDung = 0,
                    TrangThai = model.TrangThai
                };

                _context.KhuyenMai.Add(km);
                await _context.SaveChangesAsync();

                // HasDefaultValue(true): Nếu muốn tạo với TrangThai = false thì update trong transaction
                if (!model.TrangThai)
                {
                    await _context.KhuyenMai
                        .Where(k => k.MaKM == km.MaKM)
                        .ExecuteUpdateAsync(s => s.SetProperty(k => k.TrangThai, false));
                    km.TrangThai = false;
                }

                await transaction.CommitAsync();
                return (true, null, km.MaKM);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return (false, string.Format(_localizer["Admin_Promo_Error_DuplicateCode"].Value, code), null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatKhuyenMaiAsync(int maKM, KhuyenMaiAdminFormViewModel model)
        {
            var km = await _context.KhuyenMai.FindAsync(maKM);
            if (km == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            var tenKM = model.TenKM?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(tenKM))
            {
                return (false, _localizer["Validation_Required"].Value);
            }

            if (model.LoaiGiam == LoaiGiamGiaConstant.PhanTram)
            {
                if (model.GiaTriGiam <= 0 || model.GiaTriGiam > 100)
                {
                    return (false, _localizer["Admin_Promo_Error_PercentValue"].Value);
                }
            }
            else if (model.LoaiGiam == LoaiGiamGiaConstant.SoTien)
            {
                if (model.GiaTriGiam <= 0)
                {
                    return (false, _localizer["Admin_Promo_Error_AmountValue"].Value);
                }
            }

            if (model.NgayKetThuc.Date < model.NgayBatDau.Date)
            {
                return (false, _localizer["Validation_DateRangeInvalid"].Value);
            }

            if (model.SoLuotToiDa.HasValue && model.SoLuotToiDa.Value < km.SoLuotDaDung)
            {
                return (false, string.Format(_localizer["Admin_Promo_Error_MaxUsesLessThanUsed"].Value, model.SoLuotToiDa.Value, km.SoLuotDaDung));
            }

            // Nếu chưa có lượt dùng nào thì cho phép đổi Code (kiểm tra trùng)
            if (km.SoLuotDaDung == 0)
            {
                var newCode = model.Code?.Trim().ToUpperInvariant() ?? string.Empty;
                if (string.IsNullOrEmpty(newCode) || !Regex.IsMatch(newCode, @"^[A-Za-z0-9]+$"))
                {
                    return (false, _localizer["Admin_Promo_CodeHint"].Value);
                }

                if (newCode != km.Code && await _context.KhuyenMai.AnyAsync(k => k.MaKM != maKM && k.Code == newCode))
                {
                    return (false, string.Format(_localizer["Admin_Promo_Error_DuplicateCode"].Value, newCode));
                }

                km.Code = newCode;
            }
            // Nếu đã có lượt dùng: giữ nguyên km.Code

            km.TenKM = tenKM;
            km.LoaiGiam = model.LoaiGiam;
            km.GiaTriGiam = model.GiaTriGiam;
            km.GiamToiDa = model.LoaiGiam == LoaiGiamGiaConstant.PhanTram ? model.GiamToiDa : null;
            km.DonToiThieu = Math.Max(0, model.DonToiThieu);
            km.NgayBatDau = model.NgayBatDau.Date;
            km.NgayKetThuc = model.NgayKetThuc.Date.AddDays(1).AddTicks(-1);
            km.SoLuotToiDa = model.SoLuotToiDa;
            km.TrangThai = model.TrangThai;
            // TUYỆT ĐỐI không gán km.SoLuotDaDung

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, string.Format(_localizer["Admin_Promo_Error_DuplicateCode"].Value, km.Code));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaKhuyenMaiAsync(int maKM)
        {
            var km = await _context.KhuyenMai.FindAsync(maKM);
            if (km == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            // Kiểm tra tham chiếu từ DonHang
            bool coDonHang = await _context.DonHang.AnyAsync(d => d.MaKM == maKM);
            if (coDonHang || km.SoLuotDaDung > 0)
            {
                return (false, _localizer["Admin_Promo_Delete_HasOrders"].Value);
            }

            try
            {
                _context.KhuyenMai.Remove(km);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DatTrangThaiKhuyenMaiAsync(int maKM, bool trangThaiMoi)
        {
            var km = await _context.KhuyenMai.FindAsync(maKM);
            if (km == null)
            {
                return (false, _localizer["Common_Error"].Value);
            }

            km.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}

