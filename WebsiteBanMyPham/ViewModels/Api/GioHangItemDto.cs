namespace WebsiteBanMyPham.ViewModels.Api
{
    public class GioHangItemDto
    {
        public int MaGH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal GiaHienTai { get; set; }
        public decimal? GiaGoc { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public int SoLuongTon { get; set; }
        public bool ConHang { get; set; }
        public bool VuotTonKho { get; set; }
        public bool HopLeDeDat { get; set; }
        public string? ThongBaoTrangThai { get; set; }
    }
}

