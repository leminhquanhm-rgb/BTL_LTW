namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class DoanhThuThangExcelDto
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int SoDonHoanThanh { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class SanPhamTonKhoExcelDto
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string TenDM { get; set; } = string.Empty;
        public string TenTH { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public bool TrangThaiKinhDoanh { get; set; }
        public string CanhBao { get; set; } = string.Empty;
        public bool LaSapHet { get; set; }
    }

    public class DonHangExcelDto
    {
        public int MaDH { get; set; }
        public string MaDonHangText { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string TenNguoiNhan { get; set; } = string.Empty;
        public string SdtNguoiNhan { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public string TrangThaiDon { get; set; } = string.Empty;
        public string TenPhuongThuc { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public decimal TongThanhToan { get; set; }
        public DateTime? NgayVanDon { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
    }

    public class DonHangHoanThanhThangExcelDto
    {
        public int MaDH { get; set; }
        public string MaDonHangText { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public DateTime NgayGiaoThanhCong { get; set; }
        public decimal TongTienHang { get; set; }
        public string CodeKM { get; set; } = string.Empty;
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TenPhuongThuc { get; set; } = string.Empty;
    }
}

