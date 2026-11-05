using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DatLaiMatKhauViewModel
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

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(72, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 đến 72 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string XacNhanMatKhauMoi { get; set; } = null!;
    }
}

