using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Controllers
{
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class KhieuNaiController : Controller
    {
        private readonly IKhieuNaiService _khieuNaiService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhieuNaiController(
            IKhieuNaiService khieuNaiService,
            IStringLocalizer<SharedResource> localizer)
        {
            _khieuNaiService = khieuNaiService;
            _localizer = localizer;
        }

        [HttpGet("KhieuNai")]
        public async Task<IActionResult> Index([FromQuery] int pageIndex = 1)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = await _khieuNaiService.LayDanhSachAsync(maTK.Value, pageIndex, 10);
            return View(model);
        }

        [HttpGet("KhieuNai/Tao")]
        public async Task<IActionResult> Tao([FromQuery] int? maDH, [FromQuery] int? maSP)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var orders = await _khieuNaiService.LayDonHangVaSanPhamCuaKhachAsync(maTK.Value);

            int selectedMaDH = 0;
            if (maDH.HasValue && orders.Any(o => o.MaDH == maDH.Value))
            {
                selectedMaDH = maDH.Value;
            }
            else if (orders.Any())
            {
                selectedMaDH = orders.First().MaDH;
            }

            var vm = new TaoKhieuNaiViewModel
            {
                MaDH = selectedMaDH,
                MaSP = maSP,
                NoiDung = string.Empty,
                DanhSachDonHang = orders
            };

            return View(vm);
        }

        [HttpPost("KhieuNai/Tao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(TaoKhieuNaiViewModel model)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!ModelState.IsValid)
            {
                model.DanhSachDonHang = await _khieuNaiService.LayDonHangVaSanPhamCuaKhachAsync(maTK.Value);
                return View(model);
            }

            var (success, errorMsg, loaiLoi, maKN) = await _khieuNaiService.TaoKhieuNaiAsync(
                maTK.Value, model.MaDH, model.MaSP, model.NoiDung);

            if (success && maKN.HasValue)
            {
                TempData[ThongBaoConstant.ThanhCong] = _localizer["Complaint_SuccessSubmitted"].Value;
                return RedirectToAction("ChiTiet", new { maKN = maKN.Value });
            }

            ModelState.AddModelError(string.Empty, errorMsg ?? _localizer["Common_Error"].Value);
            model.DanhSachDonHang = await _khieuNaiService.LayDonHangVaSanPhamCuaKhachAsync(maTK.Value);
            return View(model);
        }

        [HttpGet("KhieuNai/ChiTiet/{maKN:int}")]
        public async Task<IActionResult> ChiTiet(int maKN)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = await _khieuNaiService.LayChiTietAsync(maTK.Value, maKN);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}

