using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa = null,
            int? danhMuc = null,
            int? thuongHieu = null,
            decimal? giaTu = null,
            decimal? giaDen = null,
            string? mucGia = null,
            string? khoangGia = null,
            string? sapXep = null,
            int trang = 1,
            int? pageIndex = null)
        {
            // Điều chỉnh 2: Đặt header Vary: X-Requested-With để chống lưu đệm nhầm lẫn giữa trang đầy đủ và AJAX
            Response.Headers.Vary = "X-Requested-With";

            int actualTrang = pageIndex.HasValue ? pageIndex.Value : trang;
            if (actualTrang < 1) actualTrang = 1;

            var filter = new SanPhamFilterViewModel
            {
                TuKhoa = tuKhoa,
                DanhMuc = danhMuc,
                ThuongHieu = thuongHieu,
                GiaTu = giaTu,
                GiaDen = giaDen,
                MucGia = mucGia ?? khoangGia,
                SapXep = sapXep,
                Trang = actualTrang,
                PageSize = 12
            };

            filter.ChuanHoaGia();

            var pagedList = await _sanPhamService.TimKiemAsync(filter);
            var danhMucs = await _sanPhamService.LayDanhSachDanhMucAsync();
            var thuongHieus = await _sanPhamService.LayDanhSachThuongHieuAsync();

            var viewModel = new SanPhamIndexViewModel
            {
                Filter = filter,
                DanhSachSanPham = pagedList,
                DanhSachDanhMuc = danhMucs,
                DanhSachThuongHieu = thuongHieus
            };

            ViewData["PageIndex"] = pagedList.PageIndex;
            ViewData["TotalPages"] = pagedList.TotalPages;

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return PartialView("_SanPhamGridPartial", viewModel);
            }

            return View(viewModel);
        }
    }
}

