namespace WebsiteBanMyPham.ViewModels
{
    public class GioHangItemViewModel
    {
        public int MaGH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal GiaHienTai { get; set; }
        public decimal? GiaGoc { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => GiaHienTai * SoLuong;
        public int SoLuongTon { get; set; }
        public bool TrangThaiKinhDoanh { get; set; }

        public bool ConHang => TrangThaiKinhDoanh && SoLuongTon > 0;
        public bool VuotTonKho => TrangThaiKinhDoanh && SoLuong > SoLuongTon;
        public bool HopLeDeDat => TrangThaiKinhDoanh && SoLuongTon >= SoLuong && SoLuong > 0;
        public string? ThongBaoTrangThai { get; set; }
    }

    public class GioHangViewModel
    {
        public List<GioHangItemViewModel> Items { get; set; } = new();
        public decimal TongTien => Items.Where(i => i.HopLeDeDat).Sum(i => i.ThanhTien);
        public int TongSoLuong => Items.Where(i => i.HopLeDeDat).Sum(i => i.SoLuong);
        public bool CoSanPhamKhongHopLe => Items.Any(i => !i.HopLeDeDat);
    }
}

