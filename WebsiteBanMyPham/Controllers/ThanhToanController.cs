using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Controllers
{
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    public class ThanhToanController : Controller
    {
        private readonly IDonHangService _donHangService;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ThanhToanController(
            IDonHangService donHangService,
            IConfiguration configuration,
            IStringLocalizer<SharedResource> localizer)
        {
            _donHangService = donHangService;
            _configuration = configuration;
            _localizer = localizer;
        }

        private int LayMaTaiKhoanHienTai()
        {
            var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(str, out int maTK) ? maTK : 0;
        }

        [HttpGet("ThanhToan/MoPhong/{maDH:int}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> MoPhong(int maDH)
        {
            bool choPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");
            if (!choPhepMoPhong)
            {
                return NotFound();
            }

            int maTK = LayMaTaiKhoanHienTai();
            if (maTK <= 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var donHang = await _donHangService.LayChiTietDonHangKhachAsync(maTK, maDH);
            if (donHang == null)
            {
                return NotFound();
            }

            // Điều chỉnh 9: Nếu đơn đã hủy -> chuyển về TrangChu/Index kèm TempData thông báo
            if (donHang.TrangThai == TrangThaiDonHangConstant.DaHuy)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Msg_Order_CancelledCannotPay"].Value;
                return RedirectToAction("Index", "TrangChu");
            }

            // Nếu đơn không phải phương thức Online hoặc đã thanh toán thành công -> chuyển sang trang thành công
            if (donHang.ThanhToan?.TenPT != PhuongThucThanhToanConstant.Online ||
                donHang.ThanhToan?.TrangThai == TrangThaiThanhToanConstant.DaThanhToan)
            {
                return RedirectToAction("ThanhCong", "DatHang", new { maDH });
            }

            return View(donHang);
        }

        [HttpPost("ThanhToan/XuLy")]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> XuLy(int maDH, bool thanhCong)
        {
            bool choPhepMoPhong = _configuration.GetValue<bool>("ThanhToan:ChoPhepMoPhong");
            if (!choPhepMoPhong)
            {
                return NotFound();
            }

            int maTK = LayMaTaiKhoanHienTai();
            if (maTK <= 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var (success, errorMsg) = await _donHangService.GhiNhanThanhToanOnlineMoPhongAsync(maTK, maDH, thanhCong);

            if (success)
            {
                if (thanhCong)
                {
                    TempData[ThongBaoConstant.ThanhCong] = _localizer["PaymentSim_SuccessMessage"].Value;
                    return RedirectToAction("ThanhCong", "DatHang", new { maDH });
                }

                // Điều chỉnh 2: Thanh toán thất bại -> hiển thị lại trang MoPhong dùng ModelState.AddModelError, KHÔNG dùng TempData
                var donHangFail = await _donHangService.LayChiTietDonHangKhachAsync(maTK, maDH);
                if (donHangFail == null) return NotFound();

                ModelState.AddModelError(string.Empty, _localizer["PaymentSim_FailedMessage"].Value);
                return View("MoPhong", donHangFail);
            }

            // Ghi nhận thanh toán thất bại từ service
            var donHang = await _donHangService.LayChiTietDonHangKhachAsync(maTK, maDH);
            if (donHang == null) return NotFound();

            ModelState.AddModelError(string.Empty, errorMsg ?? _localizer["Common_Error"].Value);
            return View("MoPhong", donHang);
        }
    }
}

