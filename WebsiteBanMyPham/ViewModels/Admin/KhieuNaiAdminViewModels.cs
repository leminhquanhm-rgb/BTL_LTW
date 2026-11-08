using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class KhieuNaiAdminFilterViewModel
    {
        public string? TrangThai { get; set; } = BoLocTrangThaiConstant.DangMo;
        public string? Keyword { get; set; }
        public int Trang { get; set; } = 1;
    }

    public class KhieuNaiAdminStatusCountViewModel
    {
        public int TatCa { get; set; }
        public int DangMo { get; set; }
        public int ChoXuLy { get; set; }
        public int DangXuLy { get; set; }
        public int DaGiaiQuyet { get; set; }
        public int TuChoi { get; set; }
    }

    public class KhieuNaiAdminListItemViewModel
    {
        public int MaKN { get; set; }
        public int MaTK { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;
        public int? MaDH { get; set; }
        public int? MaSP { get; set; }
        public string? TenSP { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int SoNgayCho => (int)Math.Max(0, (DateTime.Now - NgayGui).TotalDays);
    }

    public class KhieuNaiAdminDetailViewModel
    {
        public int MaKN { get; set; }
        public int MaTK { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;
        public string? SdtKhachHang { get; set; }

        // Đơn hàng
        public int? MaDH { get; set; }
        public string? TrangThaiDonHang { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }

        // Sản phẩm
        public int? MaSP { get; set; }
        public string? TenSP { get; set; }
        public string? AnhSP { get; set; }

        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Xử lý & Phản hồi
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
        public int? MaTKXuLy { get; set; }
        public string? TenAdminXuLy { get; set; }

        public bool LaTrangThaiCuoi => TrangThai == TrangThaiKhieuNaiConstant.DaGiaiQuyet || TrangThai == TrangThaiKhieuNaiConstant.TuChoi;
    }

    public class KhieuNaiAdminProcessFormModel
    {
        public int Id { get; set; }
        public string TrangThaiCu { get; set; } = string.Empty;
        public string TrangThaiMoi { get; set; } = string.Empty;
        public string? PhanHoi { get; set; }
        public string? ReturnUrl { get; set; }
    }
}

