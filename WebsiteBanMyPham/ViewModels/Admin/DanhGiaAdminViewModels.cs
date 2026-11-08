using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class DanhGiaAdminFilterViewModel
    {
        public string? TrangThai { get; set; } = TrangThaiDanhGiaConstant.ChoDuyet;
        public byte? SoSao { get; set; }
        public string? Keyword { get; set; }
        public int Trang { get; set; } = 1;
    }

    public class DanhGiaAdminStatusCountViewModel
    {
        public int TatCa { get; set; }
        public int ChoDuyet { get; set; }
        public int DaDuyet { get; set; }
        public int TuChoi { get; set; }
    }

    public class DanhGiaAdminListItemViewModel
    {
        public int MaDG { get; set; }
        public int MaCTDH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string? AnhSP { get; set; }
        public int MaDH { get; set; }
        public int MaTK { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;
        public byte SoSao { get; set; }
        public string? NoiDung { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
    }
}

