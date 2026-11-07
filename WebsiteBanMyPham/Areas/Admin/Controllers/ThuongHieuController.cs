using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class ThuongHieuController : AdminBaseController
    {
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ThuongHieuController(
            IThuongHieuService thuongHieuService,
            IFileUploadService fileUploadService,
            IStringLocalizer<SharedResource> localizer)
        {
            _thuongHieuService = thuongHieuService;
            _fileUploadService = fileUploadService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? trangThai, int trang = 1, int pageSize = 10)
        {
            var pagedResult = await _thuongHieuService.LayDanhSachAdminAsync(keyword, trangThai, trang, pageSize);

            var items = new List<ThuongHieuListItemViewModel>();
            foreach (var item in pagedResult.Items)
            {
                int dangBan = await _thuongHieuService.LaySoSanPhamCuaThuongHieuAsync(item.MaTH);
                items.Add(new ThuongHieuListItemViewModel
                {
                    MaTH = item.MaTH,
                    TenTH = item.TenTH,
                    Logo = item.Logo,
                    MoTa = item.MoTa,
                    TrangThai = item.TrangThai,
                    SoLuongSanPham = item.SoLuongSanPham,
                    SoSanPhamDangBan = dangBan
                });
            }

            var model = new ThuongHieuListViewModel
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
            return View(new ThuongHieuFormViewModel { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(ThuongHieuFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? uploadedFilePath = null;
            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(model.LogoFile, UploadFolderConstant.ThuongHieu);
                if (!uploadSuccess)
                {
                    ModelState.AddModelError(nameof(model.LogoFile), uploadError ?? _localizer["Upload_Error_System"].Value);
                    return View(model);
                }
                uploadedFilePath = filePath;
            }

            var (loaiLoi, _, _) = await _thuongHieuService.TaoAsync(model.TenTH, model.MoTa, model.TrangThai, uploadedFilePath);

            if (loaiLoi != LoaiLoiThuongHieu.ThanhCong)
            {
                // Điều chỉnh 4: Nếu lưu database thất bại thì XÓA file vừa upload
                if (uploadedFilePath != null)
                {
                    _fileUploadService.DeleteFile(uploadedFilePath, UploadFolderConstant.ThuongHieu);
                }

                if (loaiLoi == LoaiLoiThuongHieu.TrungTen)
                {
                    ModelState.AddModelError(nameof(model.TenTH), string.Format(_localizer["Api_Admin_Brand_DuplicateName"].Value, model.TenTH.Trim()));
                    return View(model);
                }

                if (loaiLoi == LoaiLoiThuongHieu.DuLieuKhongHopLe)
                {
                    ModelState.AddModelError(string.Empty, _localizer["Validation_Required"].Value);
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, _localizer["Upload_Error_System"].Value);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Brand_CreateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (id <= 0) return NotFound();

            var th = await _thuongHieuService.LayChiTietAdminAsync(id);
            if (th == null) return NotFound();

            var model = new ThuongHieuFormViewModel
            {
                MaTH = th.MaTH,
                TenTH = th.TenTH,
                MoTa = th.MoTa,
                LogoHienTai = th.Logo,
                TrangThai = th.TrangThai,
                SoLuongSanPham = th.SoLuongSanPham
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, ThuongHieuFormViewModel model)
        {
            if (id <= 0 || id != model.MaTH) return BadRequest();

            var existing = await _thuongHieuService.LayChiTietAdminAsync(id);
            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.LogoHienTai = existing.Logo;
                return View(model);
            }

            string? oldLogo = existing.Logo;
            string? newLogo = oldLogo;
            bool capNhatLogo = false;
            string? newlyUploadedLogo = null;

            if (model.XoaLogo)
            {
                newLogo = null;
                capNhatLogo = true;
            }
            else if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var (uploadSuccess, filePath, _, uploadError) = await _fileUploadService.UploadImageAsync(model.LogoFile, UploadFolderConstant.ThuongHieu);
                if (!uploadSuccess)
                {
                    model.LogoHienTai = oldLogo;
                    ModelState.AddModelError(nameof(model.LogoFile), uploadError ?? _localizer["Upload_Error_System"].Value);
                    return View(model);
                }
                newlyUploadedLogo = filePath;
                newLogo = newlyUploadedLogo;
                capNhatLogo = true;
            }

            var (loaiLoi, _) = await _thuongHieuService.CapNhatAsync(id, model.TenTH, model.MoTa, model.TrangThai, newLogo, capNhatLogo);

            if (loaiLoi != LoaiLoiThuongHieu.ThanhCong)
            {
                // Điều chỉnh 4: Nếu lưu DB thất bại thì xóa file vừa upload
                if (newlyUploadedLogo != null)
                {
                    _fileUploadService.DeleteFile(newlyUploadedLogo, UploadFolderConstant.ThuongHieu);
                }

                model.LogoHienTai = oldLogo;

                if (loaiLoi == LoaiLoiThuongHieu.KhongTimThay)
                {
                    return NotFound();
                }

                if (loaiLoi == LoaiLoiThuongHieu.TrungTen)
                {
                    ModelState.AddModelError(nameof(model.TenTH), string.Format(_localizer["Api_Admin_Brand_DuplicateName"].Value, model.TenTH.Trim()));
                    return View(model);
                }

                if (loaiLoi == LoaiLoiThuongHieu.DuLieuKhongHopLe)
                {
                    ModelState.AddModelError(string.Empty, _localizer["Validation_Required"].Value);
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, _localizer["Upload_Error_System"].Value);
                return View(model);
            }

            // Điều chỉnh 4: Chỉ xóa logo cũ SAU KHI lưu database thành công
            if (capNhatLogo && !string.IsNullOrWhiteSpace(oldLogo) && oldLogo != newLogo)
            {
                _fileUploadService.DeleteFile(oldLogo, UploadFolderConstant.ThuongHieu);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Brand_UpdateSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatTat(int id)
        {
            if (id <= 0) return NotFound();

            var th = await _thuongHieuService.LayChiTietAdminAsync(id);
            if (th == null) return NotFound();

            var (loaiLoi, _) = await _thuongHieuService.CapNhatAsync(id, th.TenTH, th.MoTa, !th.TrangThai);
            if (loaiLoi == LoaiLoiThuongHieu.KhongTimThay) return NotFound();

            TempData["SuccessMessage"] = _localizer["Admin_Brand_ToggleSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            if (id <= 0) return NotFound();

            var existing = await _thuongHieuService.LayChiTietAdminAsync(id);
            if (existing == null) return NotFound();

            var (loaiLoi, soLuongSP) = await _thuongHieuService.XoaAsync(id);

            if (loaiLoi == LoaiLoiThuongHieu.KhongTimThay)
            {
                return NotFound();
            }

            if (loaiLoi == LoaiLoiThuongHieu.DangCoSanPhamThamChieu)
            {
                TempData["ErrorMessage"] = string.Format(_localizer["Api_Admin_Brand_HasProducts"].Value, soLuongSP);
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrWhiteSpace(existing.Logo))
            {
                _fileUploadService.DeleteFile(existing.Logo, UploadFolderConstant.ThuongHieu);
            }

            TempData["SuccessMessage"] = _localizer["Admin_Brand_DeleteSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}

