using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class TonKhoController : AdminBaseController
    {
        private readonly ISanPhamAdminService _sanPhamAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TonKhoController(
            ISanPhamAdminService sanPhamAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _sanPhamAdminService = sanPhamAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(TonKhoFilterViewModel filter)
        {
            var pagedResult = await _sanPhamAdminService.LayDanhSachTonKhoAsync(filter);
            var dropdowns = await _sanPhamAdminService.LayDuLieuFormThemAsync();

            ViewBag.DanhMucs = dropdowns.DanhMucs;
            ViewBag.Filter = filter;

            return View(pagedResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DieuChinh(DieuChinhTonKhoRequestViewModel model)
        {
            if (model.MaSP <= 0) return BadRequest();

            var (success, errorMessage, _) = await _sanPhamAdminService.DieuChinhTonKhoAsync(model.MaSP, model.Delta);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Inventory_Adjust_Success"].Value;
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatNguong(CapNhatNguongCanhBaoRequestViewModel model)
        {
            if (model.MaSP <= 0) return BadRequest();

            var (success, errorMessage) = await _sanPhamAdminService.CapNhatNguongCanhBaoAsync(model.MaSP, model.NguongCanhBao);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }
            else
            {
                TempData["SuccessMessage"] = _localizer["Admin_Inventory_UpdateThreshold_Success"].Value;
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

