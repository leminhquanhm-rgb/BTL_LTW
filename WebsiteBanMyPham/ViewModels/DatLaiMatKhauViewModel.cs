using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DatLaiMatKhauViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [EmailAddress(ErrorMessage = "Validation_Email")]
        [StringLength(150, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Validation_OtpLength")]
        [Display(Name = "Label_MaOtp")]
        public string Otp { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(72, MinimumLength = 6, ErrorMessage = "Validation_StringLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Label_MatKhauMoi")]
        public string MatKhauMoi { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Validation_ComparePassword")]
        [Display(Name = "Label_XacNhanMatKhauMoi")]
        public string XacNhanMatKhauMoi { get; set; } = null!;
    }
}
