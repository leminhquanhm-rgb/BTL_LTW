namespace WebsiteBanMyPham.ViewModels.Api
{
    public class ChiTietSanPhamDonHangDto
    {
        public int MaCTDH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public bool DaDanhGia { get; set; }
        public string? TrangThaiDanhGia { get; set; }
    }

    public class DonHangKhachItemDto
    {
        public int MaDH { get; set; }
        public string MaDonHangText { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
        public string TrangThai { get; set; } = null!;
        public string TenPhuongThuc { get; set; } = null!;
        public string? TrangThaiThanhToan { get; set; }
        public int SoLuongSanPham { get; set; }
        public List<ChiTietSanPhamDonHangDto> SanPham { get; set; } = new();
    }

    public class DonHangKhachListDto
    {
        public List<DonHangKhachItemDto> Items { get; set; } = new();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class LichSuDonHangApiDto
    {
        public int MaLS { get; set; }
        public string? TrangThaiCu { get; set; }
        public string TrangThaiMoi { get; set; } = null!;
        public DateTime NgayDoi { get; set; }
    }

    public class ThanhToanApiDto
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

    public class DonHangKhachChiTietDto
    {
        public int MaDH { get; set; }
        public string MaDonHangText { get; set; } = null!;
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
        public bool CoTheHuy { get; set; }

        public ThanhToanApiDto? ThanhToan { get; set; }
        public List<ChiTietSanPhamDonHangDto> ChiTiet { get; set; } = new();
        public List<LichSuDonHangApiDto> LichSu { get; set; } = new();
    }

    public class HuyDonHangResponseDto
    {
        public int MaDH { get; set; }
        public string TrangThai { get; set; } = null!;
        public string ThongBao { get; set; } = null!;
    }
}

