namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class KhachHangAdminFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }
        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class KhachHangAdminListItemViewModel
    {
        public int MaTK { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
        public int SoDonHang { get; set; }
        public decimal TongChiTieu { get; set; }
        public bool TrangThai { get; set; }
    }

    public class KhachHangDonHangGanNhatViewModel
    {
        public int MaDH { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? TrangThaiThanhToan { get; set; }
        public string? TrangThaiHoanTien { get; set; }
    }

    public class KhachHangAdminDetailViewModel
    {
        public int MaTK { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; }

        public int TongSoDon { get; set; }
        public int SoDonHoanThanh { get; set; }
        public decimal TongChiTieu { get; set; }

        public List<KhachHangDonHangGanNhatViewModel> DonHangGanNhat { get; set; } = new();
    }
}

