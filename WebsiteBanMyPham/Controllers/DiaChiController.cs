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
    public class DiaChiController : Controller
    {
        private readonly IDiaChiService _diaChiService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<DiaChiController> _logger;

        public DiaChiController(
            IDiaChiService diaChiService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<DiaChiController> logger)
        {
            _diaChiService = diaChiService;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpGet("DiaChi")]
        public async Task<IActionResult> Index()
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var list = await _diaChiService.LayDanhSachDiaChiAsync(maTK.Value);
            var vm = new DiaChiDanhSachViewModel
            {
                DanhSach = list,
                GioiHanToiDa = DiaChiConstant.ToiDaDiaChi
            };

            return View(vm);
        }

        [HttpGet("DiaChi/Them")]
        public async Task<IActionResult> Them()
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var list = await _diaChiService.LayDanhSachDiaChiAsync(maTK.Value);
            if (list.Count >= DiaChiConstant.ToiDaDiaChi)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Msg_Address_LimitExceeded"].Value;
                return RedirectToAction("Index");
            }

            var vm = new DiaChiFormViewModel
            {
                LaDiaChiDauTien = list.Count == 0,
                LaMacDinh = list.Count == 0
            };

            return View(vm);
        }

        [HttpPost("DiaChi/Them")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(DiaChiFormViewModel model)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!ModelState.IsValid)
            {
                var list = await _diaChiService.LayDanhSachDiaChiAsync(maTK.Value);
                model.LaDiaChiDauTien = list.Count == 0;
                if (model.LaDiaChiDauTien) model.LaMacDinh = true;
                return View(model);
            }

            var (hopLe, thongBaoLoi, _) = await _diaChiService.ThemDiaChiAsync(maTK.Value, model);
            if (!hopLe)
            {
                ModelState.AddModelError(string.Empty, thongBaoLoi ?? _localizer["Common_Error"].Value);
                var list = await _diaChiService.LayDanhSachDiaChiAsync(maTK.Value);
                model.LaDiaChiDauTien = list.Count == 0;
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Address_AddSuccess"].Value;
            return RedirectToAction("Index");
        }

        [HttpGet("DiaChi/Sua/{id:int}")]
        public async Task<IActionResult> Sua(int id)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var dc = await _diaChiService.LayChiTietAsync(id, maTK.Value);
            if (dc == null)
            {
                return NotFound();
            }

            var vm = new DiaChiFormViewModel
            {
                MaDC = dc.MaDC,
                TenNguoiNhan = dc.TenNguoiNhan,
                SoDienThoai = dc.SoDienThoai,
                DiaChi = dc.DiaChi,
                LaMacDinh = dc.LaMacDinh,
                DangLaMacDinh = dc.LaMacDinh
            };

            return View(vm);
        }

        [HttpPost("DiaChi/Sua/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, DiaChiFormViewModel model)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            model.MaDC = id;

            if (!ModelState.IsValid)
            {
                var existing = await _diaChiService.LayChiTietAsync(id, maTK.Value);
                if (existing == null) return NotFound();
                model.DangLaMacDinh = existing.LaMacDinh;
                return View(model);
            }

            var (hopLe, thongBaoLoi) = await _diaChiService.SuaDiaChiAsync(maTK.Value, id, model);
            if (!hopLe)
            {
                if (thongBaoLoi == _localizer["Msg_Address_NotFound"].Value)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, thongBaoLoi ?? _localizer["Common_Error"].Value);
                var existing = await _diaChiService.LayChiTietAsync(id, maTK.Value);
                if (existing != null) model.DangLaMacDinh = existing.LaMacDinh;
                return View(model);
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Address_EditSuccess"].Value;
            return RedirectToAction("Index");
        }

        [HttpPost("DiaChi/Xoa/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var (hopLe, thongBaoLoi) = await _diaChiService.XoaDiaChiAsync(maTK.Value, id);
            if (!hopLe)
            {
                if (thongBaoLoi == _localizer["Msg_Address_NotFound"].Value)
                {
                    return NotFound();
                }

                TempData[ThongBaoConstant.Loi] = thongBaoLoi ?? _localizer["Common_Error"].Value;
                return RedirectToAction("Index");
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Address_DeleteSuccess"].Value;
            return RedirectToAction("Index");
        }

        [HttpPost("DiaChi/DatMacDinh/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatMacDinh(int id)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var (hopLe, thongBaoLoi) = await _diaChiService.DatMacDinhAsync(maTK.Value, id);
            if (!hopLe)
            {
                if (thongBaoLoi == _localizer["Msg_Address_NotFound"].Value)
                {
                    return NotFound();
                }

                TempData[ThongBaoConstant.Loi] = thongBaoLoi ?? _localizer["Common_Error"].Value;
                return RedirectToAction("Index");
            }

            TempData[ThongBaoConstant.ThanhCong] = _localizer["Msg_Address_SetDefaultSuccess"].Value;
            return RedirectToAction("Index");
        }
    }
}
