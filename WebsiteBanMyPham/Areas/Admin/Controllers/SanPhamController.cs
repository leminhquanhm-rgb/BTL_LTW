using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class SanPhamController : AdminBaseController
    {
        private readonly ISanPhamAdminService _sanPhamAdminService;
        private readonly IHtmlSanitizerService _htmlSanitizerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SanPhamController(
            ISanPhamAdminService sanPhamAdminService,
            IHtmlSanitizerService htmlSanitizerService,
            IStringLocalizer<SharedResource> localizer)
        {
            _sanPhamAdminService = sanPhamAdminService;
            _htmlSanitizerService = htmlSanitizerService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SanPhamAdminFilterViewModel filter)
        {
            var pagedResult = await _sanPhamAdminService.LayDanhSachAdminAsync(filter);
            var dropdowns = await _sanPhamAdminService.LayDuLieuFormThemAsync();

            ViewBag.DanhMucs = dropdowns.DanhMucs;
            ViewBag.ThuongHieus = dropdowns.ThuongHieus;
            ViewBag.Filter = filter;

            return View(pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Them()
        {
            var dropdowns = await _sanPhamAdminService.LayDuLieuFormThemAsync();
            ViewBag.DanhMucs = dropdowns.DanhMucs;
            ViewBag.ThuongHieus = dropdowns.ThuongHieus;

            return View(new SanPhamCreateViewModel
            {
                TrangThai = true,
                NguongCanhBao = 5,
                SoLuongTon = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(SanPhamCreateViewModel model)
        {
            if (model.GiaGoc.HasValue && model.GiaGoc.Value < model.Gia)
            {
                ModelState.AddModelError(nameof(model.GiaGoc), _localizer["Admin_Product_Validation_OriginalPrice"].Value);
            }

            if (!ModelState.IsValid)
            {
                var dropdowns = await _sanPhamAdminService.LayDuLieuFormThemAsync();
                ViewBag.DanhMucs = dropdowns.DanhMucs;
                ViewBag.ThuongHieus = dropdowns.ThuongHieus;
                return View(model);
            }

            var (success, errorMessage, _) = await _sanPhamAdminService.TaoSanPhamAsync(model, model.AnhFiles);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Upload_Error_System"].Value);
                var dropdowns = await _sanPhamAdminService.LayDuLieuFormThemAsync();
                ViewBag.DanhMucs = dropdowns.DanhMucs;
                ViewBag.ThuongHieus = dropdowns.ThuongHieus;
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Product_CreateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var detail = await _sanPhamAdminService.LayChiTietAdminAsync(id);
            if (detail == null) return NotFound();

            var dropdowns = await _sanPhamAdminService.LayDuLieuFormSuaAsync(id);
            ViewBag.DanhMucs = dropdowns.DanhMucs;
            ViewBag.ThuongHieus = dropdowns.ThuongHieus;

            // Điều chỉnh 6: MoTa đưa vào form phải qua IHtmlSanitizerService.Sanitize trước
            var model = new SanPhamEditViewModel
            {
                MaSP = detail.MaSP,
                TenSP = detail.TenSP,
                MoTa = _htmlSanitizerService.Sanitize(detail.MoTa),
                Gia = detail.Gia,
                GiaGoc = detail.GiaGoc,
                NguongCanhBao = detail.NguongCanhBao,
                TrangThai = detail.TrangThai,
                MaDM = detail.MaDM,
                MaTH = detail.MaTH,
                SoLuongTon = detail.SoLuongTon,
                NgayTao = detail.NgayTao,
                DanhSachAnh = detail.DanhSachAnh
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, SanPhamEditViewModel model)
        {
            if (id <= 0 || id != model.MaSP) return BadRequest();

            if (model.GiaGoc.HasValue && model.GiaGoc.Value < model.Gia)
            {
                ModelState.AddModelError(nameof(model.GiaGoc), _localizer["Admin_Product_Validation_OriginalPrice"].Value);
            }

            if (!ModelState.IsValid)
            {
                var dropdowns = await _sanPhamAdminService.LayDuLieuFormSuaAsync(id);
                ViewBag.DanhMucs = dropdowns.DanhMucs;
                ViewBag.ThuongHieus = dropdowns.ThuongHieus;

                var detail = await _sanPhamAdminService.LayChiTietAdminAsync(id);
                if (detail != null)
                {
                    model.SoLuongTon = detail.SoLuongTon;
                    model.NgayTao = detail.NgayTao;
                    model.DanhSachAnh = detail.DanhSachAnh;
                }

                return View(model);
            }

            var (success, errorMessage) = await _sanPhamAdminService.CapNhatSanPhamAsync(id, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Upload_Error_System"].Value);
                var dropdowns = await _sanPhamAdminService.LayDuLieuFormSuaAsync(id);
                ViewBag.DanhMucs = dropdowns.DanhMucs;
                ViewBag.ThuongHieus = dropdowns.ThuongHieus;

                var detail = await _sanPhamAdminService.LayChiTietAdminAsync(id);
                if (detail != null)
                {
                    model.SoLuongTon = detail.SoLuongTon;
                    model.NgayTao = detail.NgayTao;
                    model.DanhSachAnh = detail.DanhSachAnh;
                }

                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Product_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatTat(int id)
        {
            if (id <= 0) return NotFound();

            var (success, errorMessage) = await _sanPhamAdminService.ChuyenTrangThaiKinhDoanhAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Product_ToggleSuccess"].Value;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemAnh(int id, IFormFile file)
        {
            if (id <= 0) return NotFound();

            var (success, errorMessage, _) = await _sanPhamAdminService.ThemAnhAsync(id, file);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Product_Images_AddSuccess"].Value;
            }

            return RedirectToAction(nameof(Sua), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatAnhChinh(int id, int maAnh)
        {
            if (id <= 0 || maAnh <= 0) return BadRequest();

            var (success, errorMessage) = await _sanPhamAdminService.DatAnhChinhAsync(id, maAnh);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Product_Images_SetPrimarySuccess"].Value;
            }

            return RedirectToAction(nameof(Sua), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaAnh(int id, int maAnh)
        {
            if (id <= 0 || maAnh <= 0) return BadRequest();

            var (success, errorMessage) = await _sanPhamAdminService.XoaAnhAsync(id, maAnh);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Product_Images_DeleteSuccess"].Value;
            }

            return RedirectToAction(nameof(Sua), new { id });
        }
    }
}

