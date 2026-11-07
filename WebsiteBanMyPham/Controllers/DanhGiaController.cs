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
    public class DanhGiaController : Controller
    {
        private readonly IDanhGiaService _danhGiaService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DanhGiaController(
            IDanhGiaService danhGiaService,
            IStringLocalizer<SharedResource> localizer)
        {
            _danhGiaService = danhGiaService;
            _localizer = localizer;
        }

        [HttpGet("DanhGia/Tao")]
        public async Task<IActionResult> Tao([FromQuery] int maCTDH)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var info = await _danhGiaService.LayThongTinDanhGiaAsync(maTK.Value, maCTDH);
            if (info == null)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Review_Error_NotFoundOrNotOwned"].Value;
                return RedirectToAction("Index", "DonHang");
            }

            if (!info.CoTheDanhGia)
            {
                TempData[ThongBaoConstant.Loi] = info.LyDoKhongTheDanhGia ?? _localizer["Review_Error_NotCompleted"].Value;
                return RedirectToAction("ChiTiet", "DonHang", new { maDH = info.MaDH });
            }

            var vm = new TaoDanhGiaViewModel
            {
                MaCTDH = info.MaCTDH,
                MaDH = info.MaDH,
                MaSP = info.MaSP,
                TenSP = info.TenSP,
                DuongDanAnh = info.DuongDanAnh,
                SoSao = 5,
                NoiDung = string.Empty
            };

            return View(vm);
        }

        [HttpPost("DanhGia/Tao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(TaoDanhGiaViewModel model)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!ModelState.IsValid)
            {
                var info = await _danhGiaService.LayThongTinDanhGiaAsync(maTK.Value, model.MaCTDH);
                if (info != null)
                {
                    model.TenSP = info.TenSP;
                    model.DuongDanAnh = info.DuongDanAnh;
                    model.MaDH = info.MaDH;
                    model.MaSP = info.MaSP;
                }
                return View(model);
            }

            var (success, errorMsg, loaiLoi) = await _danhGiaService.TaoDanhGiaAsync(
                maTK.Value, model.MaCTDH, model.SoSao, model.NoiDung);

            if (success)
            {
                TempData[ThongBaoConstant.ThanhCong] = _localizer["Review_SuccessSubmitted"].Value;
                return RedirectToAction("ChiTiet", "DonHang", new { maDH = model.MaDH });
            }

            ModelState.AddModelError(string.Empty, errorMsg ?? _localizer["Common_Error"].Value);

            var reloadInfo = await _danhGiaService.LayThongTinDanhGiaAsync(maTK.Value, model.MaCTDH);
            if (reloadInfo != null)
            {
                model.TenSP = reloadInfo.TenSP;
                model.DuongDanAnh = reloadInfo.DuongDanAnh;
                model.MaDH = reloadInfo.MaDH;
                model.MaSP = reloadInfo.MaSP;
            }

            return View(model);
        }
    }
}

