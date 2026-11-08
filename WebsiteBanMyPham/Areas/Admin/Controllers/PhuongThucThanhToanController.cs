using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class PhuongThucThanhToanController : AdminBaseController
    {
        private readonly IThanhToanAdminService _thanhToanAdminService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PhuongThucThanhToanController(
            IThanhToanAdminService thanhToanAdminService,
            IStringLocalizer<SharedResource> localizer)
        {
            _thanhToanAdminService = thanhToanAdminService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _thanhToanAdminService.LayDanhSachPhuongThucAsync();
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var pt = await _thanhToanAdminService.LayChiTietPhuongThucAsync(id);
            if (pt == null) return NotFound();

            var model = new PhuongThucThanhToanEditViewModel
            {
                MaPT = pt.MaPT,
                TenPT = pt.TenPT,
                MoTa = pt.MoTa,
                TrangThai = pt.TrangThai
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, PhuongThucThanhToanEditViewModel model)
        {
            if (id <= 0 || id != model.MaPT) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _thanhToanAdminService.CapNhatPhuongThucAsync(id, model.MoTa);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? _localizer["Upload_Error_System"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_PaymentMethod_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatTrangThai(int id, bool trangThaiMoi)
        {
            if (id <= 0) return BadRequest();

            var (success, errorMessage) = await _thanhToanAdminService.DatTrangThaiPhuongThucAsync(id, trangThaiMoi);
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["Admin_PaymentMethod_ToggleSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? _localizer["Upload_Error_System"].Value;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

