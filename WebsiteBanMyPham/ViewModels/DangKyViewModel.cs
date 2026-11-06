using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DangKyViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_HoTen")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [EmailAddress(ErrorMessage = "Validation_Email")]
        [StringLength(150, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_Email")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Validation_Phone")]
        [StringLength(15, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_SoDienThoai")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_DiaChi")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(72, MinimumLength = 6, ErrorMessage = "Validation_StringLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Label_MatKhau")]
        public string MatKhau { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Validation_ComparePassword")]
        [Display(Name = "Label_XacNhanMatKhau")]
        public string XacNhanMatKhau { get; set; } = null!;
    }
}
