using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class TaiKhoanController : AdminBaseController
    {
        private readonly ITaiKhoanAdminService _taiKhoanAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TaiKhoanController(
            ITaiKhoanAdminService taiKhoanAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _taiKhoanAdminService = taiKhoanAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(KhachHangAdminFilterViewModel filter)
        {
            var pagedResult = await _taiKhoanAdminService.LayDanhSachKhachHangAsync(filter);
            ViewBag.Filter = filter;
            return View(pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _taiKhoanAdminService.LayChiTietKhachHangAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatTrangThai(int id, bool trangThaiMoi, string? returnUrl)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _taiKhoanAdminService.DatTrangThaiKhachHangAsync(id, trangThaiMoi);
            if (success)
            {
                TempData["SuccessMessage"] = trangThaiMoi
                    ? _localizer["Admin_Customer_UnlockSuccess"].Value
                    : _localizer["Admin_Customer_LockSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

