using WebsiteBanMyPham.Models;

namespace WebsiteBanMyPham.Helpers
{
    public static class DonHangQueryExtensions
    {
        /// <summary>
        /// Bóc tách từ khóa tìm kiếm nếu có dạng mã đơn ("DH000003", "dh3", "#3", "3").
        /// Trả về số nguyên mã đơn nếu hợp lệ, ngược lại null.
        /// Dùng chung cho DonHang, ThongKe xuất Excel và ThanhToan.
        /// </summary>
        public static int? BocTachMaDonHang(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var cleaned = keyword.Trim();
            if (cleaned.StartsWith("#")) cleaned = cleaned.Substring(1).Trim();
            if (cleaned.StartsWith("dh", StringComparison.OrdinalIgnoreCase)) cleaned = cleaned.Substring(2).Trim();
            cleaned = cleaned.TrimStart('-', ' ', '_');

            if (int.TryParse(cleaned, out int parsedId) && parsedId > 0)
            {
                return parsedId;
            }

            return null;
        }

        /// <summary>
        /// Lọc danh sách đơn hàng cho Admin theo ngày và từ khóa (bỏ qua trạng thái).
        /// Dùng để tính toán số lượng đơn của 5 trạng thái cho thẻ lọc.
        /// </summary>
        public static IQueryable<DonHang> LocDonHangAdminKhongTrangThai(
            this IQueryable<DonHang> query,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? keyword)
        {
            if (tuNgay.HasValue)
            {
                var fromDate = tuNgay.Value.Date;
                query = query.Where(d => d.NgayDat >= fromDate);
            }

            if (denNgay.HasValue)
            {
                var toDate = denNgay.Value.Date.AddDays(1);
                query = query.Where(d => d.NgayDat < toDate);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                var kwLower = kw.ToLower();
                var maDHParsed = BocTachMaDonHang(kw);

                if (maDHParsed.HasValue)
                {
                    query = query.Where(d => d.MaDH == maDHParsed.Value
                                          || d.TenNguoiNhan.ToLower().Contains(kwLower)
                                          || d.SdtNguoiNhan.Contains(kw)
                                          || (d.MaTKNavigation != null && d.MaTKNavigation.HoTen.ToLower().Contains(kwLower)));
                }
                else
                {
                    query = query.Where(d => d.TenNguoiNhan.ToLower().Contains(kwLower)
                                          || d.SdtNguoiNhan.Contains(kw)
                                          || (d.MaTKNavigation != null && d.MaTKNavigation.HoTen.ToLower().Contains(kwLower)));
                }
            }

            return query;
        }

        /// <summary>
        /// Lọc danh sách đơn hàng cho Admin đầy đủ gồm trạng thái, ngày và từ khóa.
        /// Dùng chung cho LayDonHangAdminAsync và LayDuLieuDonHangExcelAsync.
        /// </summary>
        public static IQueryable<DonHang> LocDonHangAdmin(
            this IQueryable<DonHang> query,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? keyword)
        {
            query = query.LocDonHangAdminKhongTrangThai(tuNgay, denNgay, keyword);

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                var st = trangThai.Trim();
                query = query.Where(d => d.TrangThai == st);
            }

            return query;
        }
    }
}

