using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Controllers
{
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    public class DatHangController : Controller
    {
        private readonly IDatHangService _datHangService;
        private readonly IDonHangService _donHangService;
        private readonly IOrderAntiDuplicateService _antiDuplicateService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<DatHangController> _logger;

        public DatHangController(
            IDatHangService datHangService,
            IDonHangService donHangService,
            IOrderAntiDuplicateService antiDuplicateService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<DatHangController> logger)
        {
            _datHangService = datHangService;
            _donHangService = donHangService;
            _antiDuplicateService = antiDuplicateService;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpGet("DatHang")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index([FromQuery] List<int>? maGH, [FromQuery] int? maSP, [FromQuery] int? soLuong, [FromQuery] string? code)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var (hopLe, thongBaoLoi, vm) = await _datHangService.ChuanBiDatHangAsync(maTK.Value, maGH, maSP, soLuong, code);
            if (!hopLe || vm == null)
            {
                TempData[ThongBaoConstant.Loi] = thongBaoLoi ?? _localizer["Checkout_EmptySelection"].Value;
                return RedirectToAction("Index", "GioHang");
            }

            // Cấp mã chống đặt trùng mới
            vm.MaChongDatTrung = _antiDuplicateService.PhatMa(maTK.Value);
            return View(vm);
        }

        [HttpPost("DatHang/XacNhan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(DatHangFormViewModel form)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0) return RedirectToAction("DangNhap", "TaiKhoan");

            // 1. Nhánh ModelState không hợp lệ: Chưa nhận mã, giữ nguyên mã còn hiệu lực (hoặc cấp mới nếu rỗng)
            if (!ModelState.IsValid)
            {
                var prepare = await _datHangService.ChuanBiDatHangAsync(
                    maTK.Value, form.DanhSachMaGH, form.MaSPMuaNgay, form.SoLuongMuaNgay, form.CodeKM);

                if (prepare.HopLe && prepare.ViewModel != null)
                {
                    prepare.ViewModel.TenNguoiNhan = form.TenNguoiNhan ?? string.Empty;
                    prepare.ViewModel.SdtNguoiNhan = form.SdtNguoiNhan ?? string.Empty;
                    prepare.ViewModel.DiaChiGiao = form.DiaChiGiao ?? string.Empty;
                    prepare.ViewModel.GhiChu = form.GhiChu;
                    prepare.ViewModel.MaPTChon = form.MaPT;
                    prepare.ViewModel.MaChongDatTrung = !string.IsNullOrWhiteSpace(form.MaChongDatTrung)
                        ? form.MaChongDatTrung
                        : _antiDuplicateService.PhatMa(maTK.Value);

                    ModelState.Remove(nameof(DatHangFormViewModel.MaChongDatTrung));
                    return View("Index", prepare.ViewModel);
                }

                TempData[ThongBaoConstant.Loi] = prepare.ThongBaoLoi ?? _localizer["Checkout_EmptySelection"].Value;
                return RedirectToAction("Index", "GioHang");
            }

            // 2. Kiểm tra phương thức thanh toán có được phép dùng không (trả kèm TenPT)
            var checkPt = await _datHangService.KiemTraPhuongThucThanhToanAsync(form.MaPT);
            if (!checkPt.HopLe)
            {
                ModelState.AddModelError(nameof(form.MaPT), checkPt.ErrorMessage!);
                var prepare = await _datHangService.ChuanBiDatHangAsync(
                    maTK.Value, form.DanhSachMaGH, form.MaSPMuaNgay, form.SoLuongMuaNgay, form.CodeKM);

                if (prepare.HopLe && prepare.ViewModel != null)
                {
                    prepare.ViewModel.TenNguoiNhan = form.TenNguoiNhan ?? string.Empty;
                    prepare.ViewModel.SdtNguoiNhan = form.SdtNguoiNhan ?? string.Empty;
                    prepare.ViewModel.DiaChiGiao = form.DiaChiGiao ?? string.Empty;
                    prepare.ViewModel.GhiChu = form.GhiChu;
                    prepare.ViewModel.MaPTChon = form.MaPT;
                    prepare.ViewModel.MaChongDatTrung = !string.IsNullOrWhiteSpace(form.MaChongDatTrung)
                        ? form.MaChongDatTrung
                        : _antiDuplicateService.PhatMa(maTK.Value);

                    ModelState.Remove(nameof(DatHangFormViewModel.MaChongDatTrung));
                    return View("Index", prepare.ViewModel);
                }

                TempData[ThongBaoConstant.Loi] = checkPt.ErrorMessage;
                return RedirectToAction("Index", "GioHang");
            }

            // 3. Nhận mã chống đặt trùng (ThuNhanMa) TRƯỚC khi gọi TaoDonAsync
            var antiResult = _antiDuplicateService.ThuNhanMa(form.MaChongDatTrung, maTK.Value);

            // Nhánh AlreadyCompleted: Dựa vào đơn hàng THẬT của maTK qua LayChiTietDonHangKhachAsync
            if (antiResult.Status == ThuNhanMaResultType.AlreadyCompleted && antiResult.MaDH.HasValue)
            {
                var donHangThat = await _donHangService.LayChiTietDonHangKhachAsync(maTK.Value, antiResult.MaDH.Value);
                if (donHangThat?.ThanhToan?.TenPT == PhuongThucThanhToanConstant.Online &&
                    donHangThat.ThanhToan.TrangThai != TrangThaiThanhToanConstant.DaThanhToan &&
                    donHangThat.TrangThai != TrangThaiDonHangConstant.DaHuy)
                {
                    return RedirectToAction("MoPhong", "ThanhToan", new { maDH = antiResult.MaDH.Value });
                }

                return RedirectToAction("ThanhCong", new { maDH = antiResult.MaDH.Value });
            }

            // Nhánh InProgress: Đang xử lý hoặc trùng lặp
            if (antiResult.Status == ThuNhanMaResultType.InProgress)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Checkout_ProcessingOrDuplicate"].Value;
                return RedirectToAction("Index", "GioHang");
            }

            // Nhánh Invalid: Hết hạn hoặc mã không hợp lệ, chuyển về lại đúng trang đặt hàng với query ban đầu
            if (antiResult.Status == ThuNhanMaResultType.Invalid)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Checkout_SessionExpired"].Value;
                if (form.NguonDat == NguonDatHangConstant.MuaNgay || form.MaSPMuaNgay.HasValue)
                {
                    return RedirectToAction("Index", new
                    {
                        maSP = form.MaSPMuaNgay,
                        soLuong = form.SoLuongMuaNgay,
                        code = form.CodeKM
                    });
                }

                if (form.DanhSachMaGH != null && form.DanhSachMaGH.Any())
                {
                    return RedirectToAction("Index", new
                    {
                        maGH = form.DanhSachMaGH,
                        code = form.CodeKM
                    });
                }

                return RedirectToAction("Index", "GioHang");
            }

            // 4. Gọi TaoDonAsync có try/catch
            var request = new DatHangRequestViewModel
            {
                NguonDat = form.NguonDat,
                DanhSachMaGH = form.DanhSachMaGH ?? new List<int>(),
                MaSPMuaNgay = form.MaSPMuaNgay,
                SoLuongMuaNgay = form.SoLuongMuaNgay,
                TenNguoiNhan = form.TenNguoiNhan.Trim(),
                SdtNguoiNhan = form.SdtNguoiNhan.Trim(),
                DiaChiGiao = form.DiaChiGiao.Trim(),
                GhiChu = form.GhiChu?.Trim(),
                MaPT = form.MaPT,
                CodeKM = string.IsNullOrWhiteSpace(form.CodeKM) ? null : form.CodeKM.Trim()
            };

            bool success;
            string? errorMsg;
            int? maDH;

            try
            {
                (success, errorMsg, maDH) = await _donHangService.TaoDonAsync(maTK.Value, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi tạo đơn hàng cho tài khoản {MaTK}", maTK.Value);
                _antiDuplicateService.HuyMaKhiLoi(form.MaChongDatTrung);

                ModelState.AddModelError(string.Empty, _localizer["Msg_Order_CreateGeneralError"].Value);

                var reloadPrepare = await _datHangService.ChuanBiDatHangAsync(
                    maTK.Value, form.DanhSachMaGH, form.MaSPMuaNgay, form.SoLuongMuaNgay, form.CodeKM);

                if (reloadPrepare.HopLe && reloadPrepare.ViewModel != null)
                {
                    reloadPrepare.ViewModel.TenNguoiNhan = form.TenNguoiNhan;
                    reloadPrepare.ViewModel.SdtNguoiNhan = form.SdtNguoiNhan;
                    reloadPrepare.ViewModel.DiaChiGiao = form.DiaChiGiao;
                    reloadPrepare.ViewModel.GhiChu = form.GhiChu;
                    reloadPrepare.ViewModel.MaPTChon = form.MaPT;
                    reloadPrepare.ViewModel.MaChongDatTrung = _antiDuplicateService.PhatMa(maTK.Value);

                    ModelState.Remove(nameof(DatHangFormViewModel.MaChongDatTrung));
                    return View("Index", reloadPrepare.ViewModel);
                }

                TempData[ThongBaoConstant.Loi] = _localizer["Msg_Order_CreateGeneralError"].Value;
                return RedirectToAction("Index", "GioHang");
            }

            // Tạo đơn thành công
            if (success && maDH.HasValue)
            {
                _antiDuplicateService.CapNhatKetQua(form.MaChongDatTrung, maDH.Value);

                if (checkPt.TenPT == PhuongThucThanhToanConstant.Online)
                {
                    return RedirectToAction("MoPhong", "ThanhToan", new { maDH = maDH.Value });
                }

                return RedirectToAction("ThanhCong", new { maDH = maDH.Value });
            }

            // Tạo đơn thất bại từ tầng Service
            _antiDuplicateService.HuyMaKhiLoi(form.MaChongDatTrung);

            ModelState.AddModelError(string.Empty, errorMsg ?? _localizer["Common_Error"].Value);

            var failPrepare = await _datHangService.ChuanBiDatHangAsync(
                maTK.Value, form.DanhSachMaGH, form.MaSPMuaNgay, form.SoLuongMuaNgay, form.CodeKM);

            if (failPrepare.HopLe && failPrepare.ViewModel != null)
            {
                failPrepare.ViewModel.TenNguoiNhan = form.TenNguoiNhan;
                failPrepare.ViewModel.SdtNguoiNhan = form.SdtNguoiNhan;
                failPrepare.ViewModel.DiaChiGiao = form.DiaChiGiao;
                failPrepare.ViewModel.GhiChu = form.GhiChu;
                failPrepare.ViewModel.MaPTChon = form.MaPT;
                failPrepare.ViewModel.MaChongDatTrung = _antiDuplicateService.PhatMa(maTK.Value);

                ModelState.Remove(nameof(DatHangFormViewModel.MaChongDatTrung));
                return View("Index", failPrepare.ViewModel);
            }

            TempData[ThongBaoConstant.Loi] = errorMsg ?? _localizer["Common_Error"].Value;
            return RedirectToAction("Index", "GioHang");
        }

        [HttpGet("DatHang/ThanhCong/{maDH:int}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ThanhCong(int maDH)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0) return RedirectToAction("DangNhap", "TaiKhoan");

            var donHang = await _donHangService.LayChiTietDonHangKhachAsync(maTK.Value, maDH);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
    }
}
