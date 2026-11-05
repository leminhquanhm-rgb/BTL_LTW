using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class HoSoViewModel
    {
        public int MaTK { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = null!;

        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Số điện thoại không đúng định dạng (bắt đầu bằng 0, độ dài 10-15 chữ số)")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = null!;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
    }
}

