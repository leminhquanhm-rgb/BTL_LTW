using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class BannerController : AdminBaseController
    {
        private readonly IBannerAdminService _bannerAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BannerController(
            IBannerAdminService bannerAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _bannerAdminService = bannerAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _bannerAdminService.LayDanhSachAsync();
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Them()
        {
            var items = await _bannerAdminService.LayDanhSachAsync();
            var model = new BannerAdminFormViewModel
            {
                ThuTu = items.Count + 1,
                TrangThai = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(BannerAdminFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage, _) = await _bannerAdminService.ThemBannerAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Banner_AddSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _bannerAdminService.LayDeSuaAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, BannerAdminFormViewModel model)
        {
            if (id <= 0 || id != model.MaBanner) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _bannerAdminService.CapNhatBannerAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Banner_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _bannerAdminService.XoaBannerAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Banner_DeleteSuccess"].Value;
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

            var (success, errorMessage) = await _bannerAdminService.DatTrangThaiBannerAsync(id, trangThaiMoi);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Banner_ToggleSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThuTu(int id, int thuTuMoi)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _bannerAdminService.CapNhatThuTuAsync(id, thuTuMoi);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Banner_OrderSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

