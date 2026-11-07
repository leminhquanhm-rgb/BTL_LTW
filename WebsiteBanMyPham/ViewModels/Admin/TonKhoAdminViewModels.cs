namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class TonKhoFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? MaDM { get; set; }
        public string? TinhTrangTon { get; set; } // "tatCa", "sapHet", "hetHang"
        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class TonKhoListItemViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string? AnhChinh { get; set; }
        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public bool IsDangBan { get; set; }
        public string TrangThaiHienThiKey { get; set; } = string.Empty;
        public bool IsSapHet => SoLuongTon > 0 && SoLuongTon <= NguongCanhBao;
        public bool IsHetHang => SoLuongTon <= 0;
    }

    public class DieuChinhTonKhoRequestViewModel
    {
        public int MaSP { get; set; }
        public int Delta { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class CapNhatNguongCanhBaoRequestViewModel
    {
        public int MaSP { get; set; }
        public int NguongCanhBao { get; set; }
        public string? ReturnUrl { get; set; }
    }
}

