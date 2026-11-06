namespace WebsiteBanMyPham.ViewModels
{
    public class KhuyenMaiKetQuaViewModel
    {
        public bool HopLe { get; set; }
        public string? ThongBao { get; set; }
        public int? MaKM { get; set; }
        public string? Code { get; set; }
        public string? TenKM { get; set; }
        public string? LoaiGiam { get; set; }
        public decimal GiaTriGiam { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TongThanhToan => Math.Max(0, TongTienHang - TienGiam);
    }

    public class KhuyenMaiItemViewModel
    {
        public int MaKM { get; set; }
        public string Code { get; set; } = null!;
        public string TenKM { get; set; } = null!;
        public string LoaiGiam { get; set; } = null!;
        public decimal GiaTriGiam { get; set; }
        public decimal? GiamToiDa { get; set; }
        public decimal DonToiThieu { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLuotToiDa { get; set; }
        public int SoLuotDaDung { get; set; }
        public int? SoLuotConLai => SoLuotToiDa.HasValue ? Math.Max(0, SoLuotToiDa.Value - SoLuotDaDung) : null;
    }
}

