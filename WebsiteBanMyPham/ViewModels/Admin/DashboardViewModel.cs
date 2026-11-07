namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class DashboardViewModel
    {
        // 4 Thẻ chỉ số tổng quan
        public decimal TongDoanhThu { get; set; }
        public decimal DoanhThuThangHienTai { get; set; }
        public decimal DoanhThuThangTruoc { get; set; }
        public double? PhanTramThayDoiDoanhThu { get; set; } // null nếu tháng trước = 0

        public int TongDonHang { get; set; }

        public int TongSanPham { get; set; }
        public int SoLuongSanPhamDangKinhDoanh { get; set; }
        public int SoLuongSanPhamSapHet { get; set; }

        public int TongKhachHang { get; set; }
        public int SoKhachHangMoiThangNay { get; set; }

        // Biểu đồ doanh thu 6 tháng gần nhất
        public List<DoanhThuThangItem> BieuDo6Thang { get; set; } = new();
        public bool CoDuLieuDoanhThu6Thang => BieuDo6Thang.Any(b => b.DoanhThu > 0);

        // Tỷ lệ trạng thái đơn hàng (5 trạng thái)
        public List<TrangThaiDonHangItem> DanhSachTrangThaiDon { get; set; } = new();

        // Top 5 sản phẩm bán chạy
        public List<TopSanPhamBanChayItem> TopBanChay { get; set; } = new();

        // Danh sách sản phẩm sắp hết hàng
        public List<SanPhamSapHetHangItem> DanhSachSapHetHang { get; set; } = new();

        // Bộ lọc năm phục vụ xuất Excel
        public int NamDuocChon { get; set; }
        public List<int> DanhSachNam { get; set; } = new();
    }

    public class DoanhThuThangItem
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal DoanhThu { get; set; }
        public int ChieuCaoPhanTram { get; set; }
        public bool LaThangHienTai { get; set; }
    }

    public class TrangThaiDonHangItem
    {
        public string TrangThai { get; set; } = string.Empty;
        public string TenTrangThai { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public double TyLePhanTram { get; set; }
        public string CssColorClass { get; set; } = string.Empty;
        public string CssBgClass { get; set; } = string.Empty;
    }

    public class TopSanPhamBanChayItem
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string DuongDanAnh { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class SanPhamSapHetHangItem
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string DuongDanAnh { get; set; } = string.Empty;
        public int MaDM { get; set; }
        public string TenDM { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public bool TrangThai { get; set; }
        public bool LaHetHang => SoLuongTon == 0;
    }
}

