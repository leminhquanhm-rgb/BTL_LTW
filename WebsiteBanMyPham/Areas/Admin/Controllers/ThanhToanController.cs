using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class ThanhToanController : AdminBaseController
    {
        private readonly IThanhToanAdminService _thanhToanAdminService;
        private readonly IDonHangService _donHangService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ThanhToanController(
            IThanhToanAdminService thanhToanAdminService,
            IDonHangService donHangService,
            IStringLocalizer<SharedResource> localizer)
        {
            _thanhToanAdminService = thanhToanAdminService;
            _donHangService = donHangService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ThanhToanAdminFilterViewModel filter)
        {
            var pagedResult = await _thanhToanAdminService.LayDanhSachGiaoDichAsync(filter);
            var overview = await _thanhToanAdminService.LayTongQuanGiaoDichAsync();
            var phuongThucs = await _thanhToanAdminService.LayDanhSachPhuongThucAsync();

            ViewBag.Overview = overview;
            ViewBag.PhuongThucs = phuongThucs;
            ViewBag.Filter = filter;

            return View(pagedResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHoanTien(int maDH, string? returnUrl)
        {
            if (maDH <= 0) return BadRequest();

            var maTKAdmin = User.LayMaTK();
            if (!maTKAdmin.HasValue) return Challenge();

            var (success, errorMessage) = await _donHangService.XacNhanHoanTienAsync(maTKAdmin.Value, maDH);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Payment_Refund_Success"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

