using System.ComponentModel.DataAnnotations;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class ThanhToanAdminFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? MaPT { get; set; }
        public string? TrangThai { get; set; }
        public string? TrangThaiHoanTien { get; set; }
        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ThanhToanAdminListItemViewModel
    {
        public int MaTT { get; set; }
        public int MaDH { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;
        public int MaPT { get; set; }
        public string TenPT { get; set; } = string.Empty;
        public decimal SoTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiHoanTien { get; set; } = string.Empty;
        public string? MaGiaoDich { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public DateTime? NgayHoanTien { get; set; }
        public DateTime NgayTao { get; set; }

        public bool CoTheXacNhanHoanTien =>
            TrangThai == TrangThaiThanhToanConstant.DaThanhToan &&
            TrangThaiHoanTien == TrangThaiHoanTienConstant.ChoHoan;
    }

    public class ThanhToanAdminOverviewViewModel
    {
        public int SoGiaoDichChoHoan { get; set; }
        public int TongSoGiaoDich { get; set; }
        public int SoGiaoDichDaThanhToan { get; set; }
        public int SoGiaoDichChuaThanhToan { get; set; }
    }

    public class PhuongThucThanhToanAdminViewModel
    {
        public int MaPT { get; set; }
        public string TenPT { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public bool LaBatBuoc => TenPT == PhuongThucThanhToanConstant.COD;
    }

    public class PhuongThucThanhToanEditViewModel
    {
        public int MaPT { get; set; }
        public string TenPT { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Validation_MaxLength")]
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
    }
}

