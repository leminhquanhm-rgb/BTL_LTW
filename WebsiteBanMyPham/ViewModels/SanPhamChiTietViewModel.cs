namespace WebsiteBanMyPham.ViewModels
{
    public class SanPhamChiTietViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal Gia { get; set; }
        public decimal? GiaGoc { get; set; }

        public int PhanTramGiam
        {
            get
            {
                if (GiaGoc.HasValue && GiaGoc.Value > Gia && GiaGoc.Value > 0)
                {
                    return (int)Math.Round((GiaGoc.Value - Gia) / GiaGoc.Value * 100m);
                }
                return 0;
            }
        }

        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;

        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
        public bool ConHang => TrangThai && SoLuongTon > 0;

        public double DiemSaoTrungBinh { get; set; }
        public int SoDanhGia { get; set; }

        public List<string> DanhSachAnh { get; set; } = new List<string>();
        public List<DanhGiaChiTietItemViewModel> DanhGiaMoiNhat { get; set; } = new List<DanhGiaChiTietItemViewModel>();
        public List<SanPhamItemViewModel> SanPhamLienQuan { get; set; } = new List<SanPhamItemViewModel>();
    }

    public class DanhGiaChiTietItemViewModel
    {
        public int MaDG { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public byte SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
    }
}

