namespace WebsiteBanMyPham.ViewModels
{
    public class SanPhamIndexViewModel
    {
        public SanPhamFilterViewModel Filter { get; set; } = new SanPhamFilterViewModel();
        public PaginatedList<SanPhamItemViewModel> DanhSachSanPham { get; set; } = new PaginatedList<SanPhamItemViewModel>();
        public List<DanhMucItemViewModel> DanhSachDanhMuc { get; set; } = new List<DanhMucItemViewModel>();
        public List<ThuongHieuItemViewModel> DanhSachThuongHieu { get; set; } = new List<ThuongHieuItemViewModel>();
        public int TongSoKetQua => DanhSachSanPham.TotalItems;
    }
}

