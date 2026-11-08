using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class DonHangController : AdminBaseController
    {
        private readonly IDonHangService _donHangService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DonHangController(
            IDonHangService donHangService,
            IStringLocalizer<SharedResource> localizer)
        {
            _donHangService = donHangService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? keyword,
            int trang = 1,
            int pageSize = 10)
        {
            // Kiểm tra tính hợp lệ của khoảng ngày
            if (tuNgay.HasValue && denNgay.HasValue && denNgay.Value.Date < tuNgay.Value.Date)
            {
                TempData["ErrorMessage"] = _localizer["Validation_DateRangeInvalid"].Value;
                denNgay = null;
            }

            var model = await _donHangService.LayDonHangAdminAsync(trangThai, tuNgay, denNgay, keyword, trang, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _donHangService.LayChiTietDonHangAdminAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenTrangThai(int id, string trangThaiMoi, string? maVanDon)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(trangThaiMoi)) return BadRequest();

            var maTKAdmin = User.LayMaTK();
            if (!maTKAdmin.HasValue) return Challenge();

            var (success, errorMessage) = await _donHangService.CapNhatTrangThaiAsync(maTKAdmin.Value, id, trangThaiMoi, maVanDon);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_Order_UpdateStatus_Success"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }

            return RedirectToAction(nameof(ChiTiet), new { id });
        }
    }
}

