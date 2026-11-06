using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class XacMinhOtpViewModel
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
    }
}
