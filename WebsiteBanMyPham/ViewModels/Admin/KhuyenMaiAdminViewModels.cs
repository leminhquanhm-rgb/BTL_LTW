using System.ComponentModel.DataAnnotations;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class KhuyenMaiAdminFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }
        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class KhuyenMaiAdminListItemViewModel
    {
        public int MaKM { get; set; }
        public string Code { get; set; } = string.Empty;
        public string TenKM { get; set; } = string.Empty;
        public string LoaiGiam { get; set; } = LoaiGiamGiaConstant.PhanTram;
        public decimal GiaTriGiam { get; set; }
        public decimal? GiamToiDa { get; set; }
        public decimal DonToiThieu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLuotToiDa { get; set; }
        public int SoLuotDaDung { get; set; }
        public bool TrangThai { get; set; }
        public string TrangThaiTinhToan { get; set; } = string.Empty;
        public bool CoDonHangThamChieu { get; set; }

        public bool CoTheXoa => !CoDonHangThamChieu && SoLuotDaDung == 0;
    }

    public class KhuyenMaiAdminFormViewModel
    {
        public int MaKM { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(30, ErrorMessage = "Validation_MaxLength")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Admin_Promo_CodeHint")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(150, ErrorMessage = "Validation_MaxLength")]
        public string TenKM { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        public string LoaiGiam { get; set; } = LoaiGiamGiaConstant.PhanTram;

        [Range(1, double.MaxValue, ErrorMessage = "Admin_Promo_Error_AmountValue")]
        public decimal GiaTriGiam { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Validation_Range")]
        public decimal? GiamToiDa { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Validation_Range")]
        public decimal DonToiThieu { get; set; } = 0;

        [Required(ErrorMessage = "Validation_Required")]
        public DateTime NgayBatDau { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Validation_Required")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Today.AddMonths(1);

        [Range(1, int.MaxValue, ErrorMessage = "Validation_Range")]
        public int? SoLuotToiDa { get; set; }

        public int SoLuotDaDung { get; set; }

        public bool TrangThai { get; set; } = true;

        public bool CoTheDoiCode { get; set; } = true;
    }
}

