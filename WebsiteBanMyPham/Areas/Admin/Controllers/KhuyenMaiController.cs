using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class KhuyenMaiController : AdminBaseController
    {
        private readonly IKhuyenMaiAdminService _khuyenMaiAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhuyenMaiController(
            IKhuyenMaiAdminService khuyenMaiAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _khuyenMaiAdminService = khuyenMaiAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(KhuyenMaiAdminFilterViewModel filter)
        {
            var pagedResult = await _khuyenMaiAdminService.LayDanhSachAsync(filter);
            ViewBag.Filter = filter;
            return View(pagedResult);
        }

        [HttpGet]
        public IActionResult Them()
        {
            var model = new KhuyenMaiAdminFormViewModel
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddMonths(1),
                TrangThai = true,
                CoTheDoiCode = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(KhuyenMaiAdminFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage, _) = await _khuyenMaiAdminService.ThemKhuyenMaiAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Promo_AddSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _khuyenMaiAdminService.LayDeSuaAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, KhuyenMaiAdminFormViewModel model)
        {
            if (id <= 0 || id != model.MaKM) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _khuyenMaiAdminService.CapNhatKhuyenMaiAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Promo_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _khuyenMaiAdminService.XoaKhuyenMaiAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Promo_DeleteSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatTrangThai(int id, bool trangThaiMoi)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _khuyenMaiAdminService.DatTrangThaiKhuyenMaiAsync(id, trangThaiMoi);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Promo_ToggleSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

