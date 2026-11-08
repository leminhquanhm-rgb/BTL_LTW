using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Controllers
{
    public class TaiKhoanController : Controller
    {
        private const string OtpEmailTempDataKey = "OtpEmail_TempData";

        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(
            ITaiKhoanService taiKhoanService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<TaiKhoanController> logger)
        {
            _taiKhoanService = taiKhoanService;
            _localizer = localizer;
            _logger = logger;
        }

        #region 1. Đăng ký tài khoản (DangKy)

        [HttpGet]
        public IActionResult DangKy()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(VaiTroConstant.QuanTriVien))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                return RedirectToAction("Index", "TrangChu");
            }

            return View(new DangKyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(VaiTroConstant.QuanTriVien))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                return RedirectToAction("Index", "TrangChu");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.DangKyAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Msg_Account_RegisterFailed"].Value);
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Account_RegisterSuccess"].Value;
            return RedirectToAction(nameof(DangNhap));
        }

        #endregion

        #region 2. Đăng nhập (DangNhap)

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(VaiTroConstant.QuanTriVien))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                return RedirectToAction("Index", "TrangChu");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new DangNhapViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(VaiTroConstant.QuanTriVien))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                return RedirectToAction("Index", "TrangChu");
            }

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.DangNhapAsync(model);
            if (!result.Success || result.User == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Msg_Account_InvalidCredentials"].Value);
                return View(model);
            }

            var (principal, properties) = _taiKhoanService.TaoClaimsPrincipal(result.User, model.RememberMe);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            if (result.User.VaiTro == VaiTroConstant.QuanTriVien)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "TrangChu");
        }

        #endregion

        #region 3. Đăng xuất (DangXuat)

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "TrangChu");
        }

        #endregion

        #region 4. Quên mật khẩu & OTP (QuenMatKhau, XacMinhOtp, DatLaiMatKhau)

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            return View(new QuenMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.TaoOtpQuenMatKhauAsync(model.Email);

            // Lưu email vào TempData để XacMinhOtp đọc, không đưa email lên URL
            TempData[OtpEmailTempDataKey] = model.Email.Trim();
            TempData[ThongBaoConstant.ThanhCong] = result.Message ?? _localizer["Msg_Account_OtpSentGeneral"].Value;

            return RedirectToAction(nameof(XacMinhOtp));
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult XacMinhOtp()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            var email = TempData[OtpEmailTempDataKey] as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(nameof(QuenMatKhau));
            }

            // Giữ lại TempData cho bước POST tiếp theo
            TempData.Keep(OtpEmailTempDataKey);

            return View(new XacMinhOtpViewModel { Email = email.Trim() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> XacMinhOtp(XacMinhOtpViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            if (!ModelState.IsValid)
            {
                TempData.Keep(OtpEmailTempDataKey);
                return View(model);
            }

            var result = await _taiKhoanService.XacMinhOtpAsync(model);
            if (!result.Success)
            {
                TempData.Keep(OtpEmailTempDataKey);
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Msg_Account_OtpInvalidOrExpired"].Value);
                return View(model);
            }

            // OTP hợp lệ: KHÔNG redirect (để OTP không xuất hiện trên URL)
            // Trả thẳng View("DatLaiMatKhau") với DatLaiMatKhauViewModel chứa Email và Otp dạng hidden
            var datLaiModel = new DatLaiMatKhauViewModel
            {
                Email = model.Email.Trim(),
                Otp = model.Otp.Trim()
            };

            return View("DatLaiMatKhau", datLaiModel);
        }

        [HttpGet]
        public IActionResult DatLaiMatKhau()
        {
            // Truy cập GET trực tiếp vào DatLaiMatKhau thì chuyển hướng về QuenMatKhau
            return RedirectToAction(nameof(QuenMatKhau));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "TrangChu");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.DatLaiMatKhauAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Msg_Account_OtpInvalidOrExpired"].Value);
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Account_ResetPasswordSuccess"].Value;
            return RedirectToAction(nameof(DangNhap));
        }

        #endregion

        #region 5. Hồ sơ cá nhân (HoSo)

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> HoSo()
        {
            var maTk = User.LayMaTK();
            if (!maTk.HasValue)
            {
                return Challenge();
            }

            var hoSo = await _taiKhoanService.GetHoSoAsync(maTk.Value);
            if (hoSo == null)
            {
                return NotFound();
            }

            return View(hoSo);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo(HoSoViewModel model)
        {
            var maTk = User.LayMaTK();
            if (!maTk.HasValue)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                var currentProfile = await _taiKhoanService.GetHoSoAsync(maTk.Value);
                if (currentProfile != null)
                {
                    model.MaTK = currentProfile.MaTK;
                    model.Email = currentProfile.Email;
                    model.VaiTro = currentProfile.VaiTro;
                    model.NgayTao = currentProfile.NgayTao;
                }
                return View(model);
            }

            var result = await _taiKhoanService.CapNhatHoSoAsync(maTk.Value, model);
            if (!result.Success)
            {
                var currentProfile = await _taiKhoanService.GetHoSoAsync(maTk.Value);
                if (currentProfile != null)
                {
                    model.MaTK = currentProfile.MaTK;
                    model.Email = currentProfile.Email;
                    model.VaiTro = currentProfile.VaiTro;
                    model.NgayTao = currentProfile.NgayTao;
                }
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Account_UpdateProfileSuccess"].Value;
            return RedirectToAction(nameof(HoSo));
        }

        #endregion

        #region 6. Đổi mật khẩu (DoiMatKhau)

        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult DoiMatKhau()
        {
            return View(new DoiMatKhauViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            var maTk = User.LayMaTK();
            if (!maTk.HasValue)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.DoiMatKhauAsync(maTk.Value, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? _localizer["Common_Error"].Value);
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Account_ChangePasswordSuccess"].Value;
            return RedirectToAction(nameof(DoiMatKhau));
        }

        #endregion

        #region 7. Từ chối truy cập (TuChoiTruyCap - 403)

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TuChoiTruyCap()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View();
        }

        #endregion
    }
}

