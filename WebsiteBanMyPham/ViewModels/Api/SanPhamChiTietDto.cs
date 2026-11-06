namespace WebsiteBanMyPham.ViewModels.Api
{
    public class SanPhamChiTietDto
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal Gia { get; set; }
        public decimal? GiaGoc { get; set; }
        public int PhanTramGiam { get; set; }

        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;

        public int SoLuongTon { get; set; }
        public bool DangKinhDoanh { get; set; }
        public bool ConHang { get; set; }

        public double DiemSaoTrungBinh { get; set; }
        public int SoDanhGia { get; set; }

        public List<string> DanhSachAnh { get; set; } = new List<string>();
        public List<DanhGiaChiTietItemViewModel> DanhGiaMoiNhat { get; set; } = new List<DanhGiaChiTietItemViewModel>();
        public List<SanPhamDto> SanPhamLienQuan { get; set; } = new List<SanPhamDto>();
    }
}

