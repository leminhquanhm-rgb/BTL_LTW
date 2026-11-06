namespace WebsiteBanMyPham.ViewModels
{
    public class ChiTietSanPhamDonHangViewModel
    {
        public int MaCTDH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
        public bool DaDanhGia { get; set; }
    }

    public class LichSuDonHangItemViewModel
    {
        public int MaLS { get; set; }
        public string? TrangThaiCu { get; set; }
        public string TrangThaiMoi { get; set; } = null!;
        public string? NguoiThucHien { get; set; }
        public DateTime NgayDoi { get; set; }
    }

    public class ThanhToanInfoViewModel
    {
        public int MaTT { get; set; }
        public int MaPT { get; set; }
        public string TenPT { get; set; } = null!;
        public decimal SoTien { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TrangThaiHoanTien { get; set; } = null!;
        public string? MaGiaoDich { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public DateTime? NgayHoanTien { get; set; }
    }

    public class DonHangKhachItemViewModel
    {
        public int MaDH { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TenPhuongThuc { get; set; } = null!;
        public int SoLuongSanPham { get; set; }
        public List<ChiTietSanPhamDonHangViewModel> SanPham { get; set; } = new();
    }

    public class DonHangKhachListViewModel
    {
        public List<DonHangKhachItemViewModel> DonHangs { get; set; } = new();
        public string? CurrentTrangThai { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; }
    }

    public class DonHangKhachChiTietViewModel
    {
        public int MaDH { get; set; }
        public int MaTK { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenNguoiNhan { get; set; } = null!;
        public string SdtNguoiNhan { get; set; } = null!;
        public string DiaChiGiao { get; set; } = null!;
        public string? GhiChu { get; set; }
        public decimal TongTienHang { get; set; }
        public string? CodeKM { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = null!;
        public DateTime? NgayVanDon { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
        public string? MaVanDon { get; set; }

        public ThanhToanInfoViewModel? ThanhToan { get; set; }
        public List<ChiTietSanPhamDonHangViewModel> ChiTiet { get; set; } = new();
        public List<LichSuDonHangItemViewModel> LichSu { get; set; } = new();

        public bool CoTheHuy => TrangThai == Constants.TrangThaiDonHangConstant.ChoXacNhan;
    }

    public class DonHangAdminItemViewModel
    {
        public int MaDH { get; set; }
        public int MaTK { get; set; }
        public string TenKhachHang { get; set; } = null!;
        public string EmailKhachHang { get; set; } = null!;
        public string TenNguoiNhan { get; set; } = null!;
        public string SdtNguoiNhan { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TenPhuongThuc { get; set; } = null!;
        public string TrangThaiThanhToan { get; set; } = null!;
        public string TrangThaiHoanTien { get; set; } = null!;
        public DateTime? NgayVanDon { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
    }

    public class DonHangAdminListViewModel
    {
        public List<DonHangAdminItemViewModel> DonHangs { get; set; } = new();
        public string? TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; }
    }

    public class DonHangAdminChiTietViewModel
    {
        public int MaDH { get; set; }
        public int MaTK { get; set; }
        public string TenKhachHang { get; set; } = null!;
        public string EmailKhachHang { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public string TenNguoiNhan { get; set; } = null!;
        public string SdtNguoiNhan { get; set; } = null!;
        public string DiaChiGiao { get; set; } = null!;
        public string? GhiChu { get; set; }
        public decimal TongTienHang { get; set; }
        public string? CodeKM { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = null!;
        public DateTime? NgayVanDon { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
        public string? MaVanDon { get; set; }

        public ThanhToanInfoViewModel? ThanhToan { get; set; }
        public List<ChiTietSanPhamDonHangViewModel> ChiTiet { get; set; } = new();
        public List<LichSuDonHangItemViewModel> LichSu { get; set; } = new();
    }
}

