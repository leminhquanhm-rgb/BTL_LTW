using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Controllers
{
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    public class GioHangController : Controller
    {
        private readonly IGioHangService _gioHangService;
        private readonly IKhuyenMaiService _khuyenMaiService;

        public GioHangController(
            IGioHangService gioHangService,
            IKhuyenMaiService khuyenMaiService)
        {
            _gioHangService = gioHangService;
            _khuyenMaiService = khuyenMaiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var maTKStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(maTKStr, out int maTK))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = await _gioHangService.LayGioHangAsync(maTK);
            var promos = await _khuyenMaiService.LayDanhSachMaKhaDungAsync(model.TongTien);
            ViewBag.AvailablePromos = promos;

            return View(model);
        }
    }
}

