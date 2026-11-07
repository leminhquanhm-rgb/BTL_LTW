using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Controllers
{
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DonHangController : Controller
    {
        private readonly IDonHangService _donHangService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DonHangController(
            IDonHangService donHangService,
            IConfiguration configuration,
            IStringLocalizer<SharedResource> localizer)
        {
            _donHangService = donHangService;
            _configuration = configuration;
            _localizer = localizer;
        }

        [HttpGet("DonHang")]
        public async Task<IActionResult> Index([FromQuery] string? trangThai, [FromQuery] int pageIndex = 1)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = await _donHangService.LayDonHangCuaKhachAsync(maTK.Value, trangThai, pageIndex, 10);
            model.ChoPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");

            return View(model);
        }

        [HttpGet("DonHang/ChiTiet/{maDH:int}")]
        public async Task<IActionResult> ChiTiet(int maDH)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var donHang = await _donHangService.LayChiTietDonHangKhachAsync(maTK.Value, maDH);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.ChoPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");

            return View(donHang);
        }
    }
}

