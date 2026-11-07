using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Models;

namespace WebsiteBanMyPham.Helpers
{
    public static class SanPhamQueryExtensions
    {
        /// <summary>
        /// Định nghĩa duy nhất: sản phẩm đang bán = SanPham.TrangThai = 1 VÀ DanhMuc.TrangThai = 1 VÀ ThuongHieu.TrangThai = 1.
        /// Sử dụng trực tiếp trong LINQ to Entities (IQueryable).
        /// </summary>
        public static IQueryable<SanPham> DangBan(this IQueryable<SanPham> query)
        {
            return query.Where(s => s.TrangThai && s.MaDMNavigation.TrangThai && s.MaTHNavigation.TrangThai);
        }

        /// <summary>
        /// Kiểm tra sản phẩm đang bán trên entity đã nạp kèm navigation properties MaDMNavigation và MaTHNavigation.
        /// </summary>
        public static bool IsDangBan(this SanPham? sp)
        {
            if (sp == null) return false;
            return sp.TrangThai &&
                   (sp.MaDMNavigation?.TrangThai ?? false) &&
                   (sp.MaTHNavigation?.TrangThai ?? false);
        }
    }
}

