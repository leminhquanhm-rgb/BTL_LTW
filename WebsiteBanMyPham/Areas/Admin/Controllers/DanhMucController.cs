using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class DanhMucController : AdminBaseController
    {
        private readonly IDanhMucService _danhMucService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DanhMucController(
            IDanhMucService danhMucService,
            IStringLocalizer<SharedResource> localizer)
        {
            _danhMucService = danhMucService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? trangThai, int trang = 1, int pageSize = 10)
        {
            var pagedResult = await _danhMucService.LayDanhSachAdminAsync(keyword, trangThai, trang, pageSize);

            var items = new List<DanhMucListItemViewModel>();
            foreach (var item in pagedResult.Items)
            {
                int dangBan = await _danhMucService.LaySoSanPhamCuaDanhMucAsync(item.MaDM);
                items.Add(new DanhMucListItemViewModel
                {
                    MaDM = item.MaDM,
                    TenDM = item.TenDM,
                    MoTa = item.MoTa,
                    TrangThai = item.TrangThai,
                    SoLuongSanPham = item.SoLuongSanPham,
                    SoSanPhamDangBan = dangBan
                });
            }

            var model = new DanhMucListViewModel
            {
                Items = items,
                Keyword = keyword,
                TrangThai = trangThai,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems,
                TotalPages = pagedResult.TotalPages
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Them()
        {
            return View(new DanhMucFormViewModel { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(DanhMucFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (loaiLoi, _, _) = await _danhMucService.TaoAsync(model.TenDM, model.MoTa, model.TrangThai);

            if (loaiLoi == LoaiLoiDanhMuc.TrungTen)
            {
                ModelState.AddModelError(nameof(model.TenDM), string.Format(_localizer["Api_Admin_Category_DuplicateName"].Value, model.TenDM.Trim()));
                return View(model);
            }

            if (loaiLoi == LoaiLoiDanhMuc.DuLieuKhongHopLe)
            {
                ModelState.AddModelError(string.Empty, _localizer["Validation_Required"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Category_CreateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var dm = await _danhMucService.LayChiTietAdminAsync(id);
            if (dm == null) return NotFound();

            var model = new DanhMucFormViewModel
            {
                MaDM = dm.MaDM,
                TenDM = dm.TenDM,
                MoTa = dm.MoTa,
                TrangThai = dm.TrangThai,
                SoLuongSanPham = dm.SoLuongSanPham
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, DanhMucFormViewModel model)
        {
            if (id <= 0 || id != model.MaDM) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (loaiLoi, _) = await _danhMucService.CapNhatAsync(id, model.TenDM, model.MoTa, model.TrangThai);

            if (loaiLoi == LoaiLoiDanhMuc.KhongTimThay)
            {
                return NotFound();
            }

            if (loaiLoi == LoaiLoiDanhMuc.TrungTen)
            {
                ModelState.AddModelError(nameof(model.TenDM), string.Format(_localizer["Api_Admin_Category_DuplicateName"].Value, model.TenDM.Trim()));
                return View(model);
            }

            if (loaiLoi == LoaiLoiDanhMuc.DuLieuKhongHopLe)
            {
                ModelState.AddModelError(string.Empty, _localizer["Validation_Required"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Category_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatTat(int id)
        {
            if (id <= 0) return NotFound();

            var dm = await _danhMucService.LayChiTietAdminAsync(id);
            if (dm == null) return NotFound();

            var (loaiLoi, _) = await _danhMucService.CapNhatAsync(id, dm.TenDM, dm.MoTa, !dm.TrangThai);
            if (loaiLoi == LoaiLoiDanhMuc.KhongTimThay) return NotFound();

            TempData["SuccessMessage"] = _localizer["Admin_Category_ToggleSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            if (id <= 0) return NotFound();

            var (loaiLoi, soLuongSP) = await _danhMucService.XoaAsync(id);

            if (loaiLoi == LoaiLoiDanhMuc.KhongTimThay)
            {
                return NotFound();
            }

            if (loaiLoi == LoaiLoiDanhMuc.DangCoSanPhamThamChieu)
            {
                TempData["ErrorMessage"] = string.Format(_localizer["Api_Admin_Category_HasProducts"].Value, soLuongSP);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = _localizer["Admin_Category_DeleteSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}

