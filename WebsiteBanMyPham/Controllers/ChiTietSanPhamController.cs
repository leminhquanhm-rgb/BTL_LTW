using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Controllers
{
    public class ChiTietSanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;

        public ChiTietSanPhamController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var model = await _sanPhamService.LayChiTietAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            // Ghi nhận sản phẩm đã xem vào Session: chỉ ghi khi sản phẩm đang kinh doanh
            if (model.TrangThai)
            {
                var viewedIds = HttpContext.Session.GetObject<List<int>>(SessionKeyConstant.SanPhamDaXem) ?? new List<int>();
                viewedIds.Remove(model.MaSP);
                viewedIds.Insert(0, model.MaSP);
                if (viewedIds.Count > SessionKeyConstant.SoLuongLuuToiDa)
                {
                    viewedIds = viewedIds.Take(SessionKeyConstant.SoLuongLuuToiDa).ToList();
                }
                HttpContext.Session.SetObject(SessionKeyConstant.SanPhamDaXem, viewedIds);
            }

            return View(model);
        }
    }
}

