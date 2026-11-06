namespace WebsiteBanMyPham.ViewModels
{
    public class SanPhamItemViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
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

        public string? AnhChinh { get; set; }
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;

        public double DiemSaoTrungBinh { get; set; }
        public int SoDanhGia { get; set; }

        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
        public bool ConHang => TrangThai && SoLuongTon > 0;
    }
}

