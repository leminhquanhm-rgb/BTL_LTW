using Microsoft.AspNetCore.Mvc;
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

            return View(model);
        }
    }
}

