namespace WebsiteBanMyPham.ViewModels.Api
{
    public class GioHangDto
    {
        public List<GioHangItemDto> Items { get; set; } = new();
        public int TongSoLuong { get; set; }
        public decimal TongTien { get; set; }
    }
}

