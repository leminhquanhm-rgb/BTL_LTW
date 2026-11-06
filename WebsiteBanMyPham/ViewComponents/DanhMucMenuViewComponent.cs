using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Models;

namespace WebsiteBanMyPham.ViewComponents
{
    public class DanhMucMenuViewComponent : ViewComponent
    {
        private readonly WebsiteBanMyPhamContext _context;

        public DanhMucMenuViewComponent(WebsiteBanMyPhamContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewMode = "Default")
        {
            var danhMucs = await _context.DanhMuc
                .AsNoTracking()
                .Where(d => d.TrangThai)
                .OrderBy(d => d.TenDM)
                .ToListAsync();

            if (string.Equals(viewMode, "Footer", StringComparison.OrdinalIgnoreCase))
            {
                return View("Footer", danhMucs);
            }

            return View("Default", danhMucs);
        }
    }
}

