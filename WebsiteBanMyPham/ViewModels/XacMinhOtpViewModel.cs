using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class XacMinhOtpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Mã OTP phải gồm 6 chữ số")]
        [Display(Name = "Mã OTP")]
        public string Otp { get; set; } = null!;
    }
}

