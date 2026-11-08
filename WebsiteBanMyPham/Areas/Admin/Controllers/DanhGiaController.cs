using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class DanhGiaController : AdminBaseController
    {
        private readonly IDanhGiaAdminService _danhGiaAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DanhGiaController(
            IDanhGiaAdminService danhGiaAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _danhGiaAdminService = danhGiaAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DanhGiaAdminFilterViewModel filter)
        {
            var pagedResult = await _danhGiaAdminService.LayDanhSachAsync(filter);
            var counts = await _danhGiaAdminService.LaySoLuongTheoTrangThaiAsync(filter.SoSao, filter.Keyword);

            ViewBag.Filter = filter;
            ViewBag.Counts = counts;

            return View(pagedResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiCu, string trangThaiMoi, string? returnUrl)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _danhGiaAdminService.CapNhatTrangThaiAsync(id, trangThaiCu, trangThaiMoi);

            if (success)
            {
                TempData["SuccessMessage"] = trangThaiMoi == TrangThaiDanhGiaConstant.DaDuyet
                    ? _localizer["Admin_Review_ApproveSuccess"].Value
                    : _localizer["Admin_Review_RejectSuccess"].Value;
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

