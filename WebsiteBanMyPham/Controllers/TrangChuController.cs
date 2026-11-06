using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly ISanPhamService _sanPhamService;

        public TrangChuController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _sanPhamService.LayDanhSachTrangChuAsync();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
