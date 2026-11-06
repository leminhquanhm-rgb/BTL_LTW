namespace WebsiteBanMyPham.ViewModels.Api
{
    public class KhuyenMaiDto
    {
        public int MaKM { get; set; }
        public string Code { get; set; } = null!;
        public string TenKM { get; set; } = null!;
        public string LoaiGiam { get; set; } = null!;
        public decimal GiaTriGiam { get; set; }
        public decimal? GiamToiDa { get; set; }
        public decimal DonToiThieu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLuotToiDa { get; set; }
        public int SoLuotDaDung { get; set; }
        public int? SoLuotConLai => SoLuotToiDa.HasValue ? Math.Max(0, SoLuotToiDa.Value - SoLuotDaDung) : null;
    }
}

