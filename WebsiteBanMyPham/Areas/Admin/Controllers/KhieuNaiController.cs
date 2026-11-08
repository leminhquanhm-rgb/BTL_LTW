using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class KhieuNaiController : AdminBaseController
    {
        private readonly IKhieuNaiAdminService _khieuNaiAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhieuNaiController(
            IKhieuNaiAdminService khieuNaiAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _khieuNaiAdminService = khieuNaiAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(KhieuNaiAdminFilterViewModel filter)
        {
            var pagedResult = await _khieuNaiAdminService.LayDanhSachAsync(filter);
            var counts = await _khieuNaiAdminService.LaySoLuongTheoTrangThaiAsync(filter.Keyword);

            ViewBag.Filter = filter;
            ViewBag.Counts = counts;

            return View(pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _khieuNaiAdminService.LayChiTietAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyKhieuNai(KhieuNaiAdminProcessFormModel form)
        {
            if (form.Id <= 0) return BadRequest();

            int? maAdmin = User.LayMaTK();
            if (!maAdmin.HasValue)
            {
                return Unauthorized();
            }

            var (success, errorMessage) = await _khieuNaiAdminService.XuLyKhieuNaiAsync(
                form.Id, form.TrangThaiCu, form.TrangThaiMoi, form.PhanHoi, maAdmin.Value);

            if (success)
            {
                if (form.TrangThaiMoi == TrangThaiKhieuNaiConstant.DangXuLy)
                {
                    TempData["SuccessMessage"] = _localizer["Admin_Complaint_ProcessSuccess"].Value;
                }
                else if (form.TrangThaiMoi == TrangThaiKhieuNaiConstant.DaGiaiQuyet)
                {
                    TempData["SuccessMessage"] = _localizer["Admin_Complaint_ResolveSuccess"].Value;
                }
                else if (form.TrangThaiMoi == TrangThaiKhieuNaiConstant.TuChoi)
                {
                    TempData["SuccessMessage"] = _localizer["Admin_Complaint_RejectSuccess"].Value;
                }

                if (!string.IsNullOrWhiteSpace(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl))
                {
                    return Redirect(form.ReturnUrl);
                }

                return RedirectToAction(nameof(ChiTiet), new { id = form.Id });
            }

            // Khi lỗi validate hoặc lỗi nghiệp vụ: Hiện lại trang chi tiết và giữ nguyên nội dung đã gõ
            var detailModel = await _khieuNaiAdminService.LayChiTietAsync(form.Id);
            if (detailModel == null) return NotFound();

            detailModel.PhanHoi = form.PhanHoi;
            TempData["ErrorMessage"] = errorMessage ?? _localizer["Common_Error"].Value;

            return View("ChiTiet", detailModel);
        }
    }
}

