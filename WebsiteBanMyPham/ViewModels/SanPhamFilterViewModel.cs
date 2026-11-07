namespace WebsiteBanMyPham.ViewModels
{
    public class SanPhamFilterViewModel
    {
        private string? _tuKhoa;

        public string? TuKhoa
        {
            get => _tuKhoa;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _tuKhoa = null;
                }
                else
                {
                    var trimmed = value.Trim();
                    _tuKhoa = trimmed.Length > 100 ? trimmed.Substring(0, 100) : trimmed;
                }
            }
        }

        public int? DanhMuc { get; set; }
        public int? ThuongHieu { get; set; }
        public decimal? GiaTu { get; set; }
        public decimal? GiaDen { get; set; }

        public string? MucGia { get; set; }

        public string? SapXep { get; set; } = "moiNhat";

        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        /// <summary>
        /// Chuẩn hóa các khoảng giá theo quy ước:
        /// under300: giaDen = 299999
        /// 300to500: giaTu = 300000, giaDen = 500000
        /// over500: giaTu = 500001
        /// </summary>
        public void ChuanHoaGia()
        {
            if (!string.IsNullOrEmpty(MucGia))
            {
                switch (MucGia.ToLowerInvariant())
                {
                    case "under300":
                        GiaTu = null;
                        GiaDen = 299999m;
                        break;
                    case "300to500":
                        GiaTu = 300000m;
                        GiaDen = 500000m;
                        break;
                    case "over500":
                        GiaTu = 500001m;
                        GiaDen = null;
                        break;
                    case "all":
                        GiaTu = null;
                        GiaDen = null;
                        break;
                    default:
                        // Không xác định
                        break;
                }
            }
            else
            {
                if (GiaTu.HasValue && GiaTu < 0) GiaTu = 0;
                if (GiaDen.HasValue && GiaDen < 0) GiaDen = 0;
                if (GiaTu.HasValue && GiaDen.HasValue && GiaTu > GiaDen)
                {
                    var temp = GiaTu;
                    GiaTu = GiaDen;
                    GiaDen = temp;
                }
            }

            if (Trang < 1) Trang = 1;
            PageSize = Math.Clamp(PageSize, 1, 50);

            if (SapXep != "moiNhat" && SapXep != "giaTang" && SapXep != "giaGiam" && SapXep != "banChay")
            {
                SapXep = "moiNhat";
            }
        }
    }
}

