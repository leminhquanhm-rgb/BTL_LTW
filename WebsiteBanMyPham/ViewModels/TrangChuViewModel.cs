namespace WebsiteBanMyPham.ViewModels
{
    public class TrangChuViewModel
    {
        public List<BannerItemViewModel> Banners { get; set; } = new List<BannerItemViewModel>();
        public List<SanPhamItemViewModel> SanPhamMoi { get; set; } = new List<SanPhamItemViewModel>();
        public List<SanPhamItemViewModel> SanPhamBanChay { get; set; } = new List<SanPhamItemViewModel>();
        public List<KhuyenMaiItemViewModel> KhuyenMaiDangDienRa { get; set; } = new List<KhuyenMaiItemViewModel>();
        public List<DanhMucItemViewModel> DanhMucNoiBat { get; set; } = new List<DanhMucItemViewModel>();
    }
}

