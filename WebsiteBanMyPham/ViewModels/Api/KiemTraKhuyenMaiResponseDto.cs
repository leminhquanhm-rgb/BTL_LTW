namespace WebsiteBanMyPham.ViewModels.Api
{
    public class KiemTraKhuyenMaiResponseDto
    {
        public bool HopLe { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public decimal TienGiam { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TongThanhToan { get; set; }
        public string? Code { get; set; }
        public int? MaKM { get; set; }
    }
}

