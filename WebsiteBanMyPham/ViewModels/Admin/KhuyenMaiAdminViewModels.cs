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

        [Display(Name = "Admin_Promo_Col_Code")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(30, ErrorMessage = "Validation_MaxLength")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Admin_Promo_CodeHint")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Admin_Promo_Col_Name")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(150, ErrorMessage = "Validation_MaxLength")]
        public string TenKM { get; set; } = string.Empty;

        [Display(Name = "Admin_Promo_Col_Type")]
        [Required(ErrorMessage = "Validation_Required")]
        public string LoaiGiam { get; set; } = LoaiGiamGiaConstant.PhanTram;

        [Display(Name = "Admin_Promo_Col_Value")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, 1000000000, ErrorMessage = "Validation_RangeMin")]
        public decimal GiaTriGiam { get; set; }

        [Display(Name = "Admin_Promo_Col_MaxDiscount")]
        [Range(1, 1000000000, ErrorMessage = "Validation_RangeMin")]
        public decimal? GiamToiDa { get; set; }

        [Display(Name = "Admin_Promo_Col_MinOrder")]
        [Range(0, 1000000000, ErrorMessage = "Validation_RangeMin")]
        public decimal DonToiThieu { get; set; } = 0;

        [Display(Name = "Admin_Promo_StartDate")]
        [Required(ErrorMessage = "Validation_Required")]
        public DateTime NgayBatDau { get; set; } = DateTime.Today;

        [Display(Name = "Admin_Promo_EndDate")]
        [Required(ErrorMessage = "Validation_Required")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Admin_Promo_MaxUses")]
        [Range(1, 1000000, ErrorMessage = "Validation_RangeMin")]
        public int? SoLuotToiDa { get; set; }

        public int SoLuotDaDung { get; set; }

        public bool TrangThai { get; set; } = true;

        public bool CoTheDoiCode { get; set; } = true;
    }
}

