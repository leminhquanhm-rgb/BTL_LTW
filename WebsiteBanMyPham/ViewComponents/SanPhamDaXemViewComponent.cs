using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.ViewComponents
{
    public class SanPhamDaXemViewComponent : ViewComponent
    {
        private readonly WebsiteBanMyPhamContext _context;

        public SanPhamDaXemViewComponent(WebsiteBanMyPhamContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int maSPHienTai = 0)
        {
            // 1. Đọc danh sách mã sản phẩm đã xem từ Session
            var viewedIds = HttpContext.Session.GetObject<List<int>>(SessionKeyConstant.SanPhamDaXem);
            if (viewedIds == null || viewedIds.Count == 0)
            {
                return Content(string.Empty);
            }

            // 2. Bỏ sản phẩm đang xem và lấy tối đa số lượng cần hiển thị
            var targetIds = viewedIds
                .Where(id => id > 0 && id != maSPHienTai)
                .Take(SessionKeyConstant.SoLuongHienThiToiDa)
                .ToList();

            if (targetIds.Count == 0)
            {
                return Content(string.Empty);
            }

            // 3. Truy vấn các sản phẩm đang kinh doanh (.DangBan(): SP = 1, DanhMuc = 1, ThuongHieu = 1)
            var productsRaw = await _context.SanPham
                .AsNoTracking()
                .Where(s => targetIds.Contains(s.MaSP))
                .DangBan()
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    DangBan = s.TrangThai && s.MaDMNavigation.TrangThai && s.MaTHNavigation.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                })
                .ToListAsync();

            if (productsRaw.Count == 0)
            {
                return Content(string.Empty);
            }

            // 4. Lấy điểm đánh giá trung bình từ VW_DanhGia (chỉ tính DaDuyet)
            var productIds = productsRaw.Select(p => p.MaSP).ToList();
            var rawRatings = await _context.VW_DanhGia
                .AsNoTracking()
                .Where(dg => productIds.Contains(dg.MaSP) && dg.TrangThai == TrangThaiDanhGiaConstant.DaDuyet)
                .GroupBy(dg => dg.MaSP)
                .Select(g => new
                {
                    MaSP = g.Key,
                    DiemTB = g.Average(x => (double)x.SoSao),
                    SoDanhGia = g.Count()
                })
                .ToListAsync();

            var ratingDict = rawRatings.ToDictionary(
                r => r.MaSP,
                r => new { DiemTB = Math.Round(r.DiemTB, 1), r.SoDanhGia }
            );

            // 5. Ánh xạ và GIỮ ĐÚNG thứ tự đã xem trong Session
            var productDict = productsRaw.ToDictionary(p => p.MaSP);
            var orderedItems = new List<SanPhamItemViewModel>();

            foreach (var id in targetIds)
            {
                if (productDict.TryGetValue(id, out var p))
                {
                    ratingDict.TryGetValue(p.MaSP, out var r);
                    orderedItems.Add(new SanPhamItemViewModel
                    {
                        MaSP = p.MaSP,
                        TenSP = p.TenSP,
                        Gia = p.Gia,
                        GiaGoc = p.GiaGoc,
                        SoLuongTon = p.SoLuongTon,
                        TrangThai = p.DangBan,
                        MaDM = p.MaDM,
                        TenDanhMuc = p.TenDanhMuc,
                        MaTH = p.MaTH,
                        TenThuongHieu = p.TenThuongHieu,
                        AnhChinh = p.AnhChinh,
                        DiemSaoTrungBinh = r != null ? r.DiemTB : 5.0,
                        SoDanhGia = r != null ? r.SoDanhGia : 0
                    });
                }
            }

            if (orderedItems.Count == 0)
            {
                return Content(string.Empty);
            }

            return View(orderedItems);
        }
    }
}

