using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class HoSoViewModel
    {
        public int MaTK { get; set; }

        [Display(Name = "Label_Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_HoTen")]
        public string HoTen { get; set; } = null!;

        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Validation_Phone")]
        [StringLength(15, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_SoDienThoai")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_DiaChi")]
        public string? DiaChi { get; set; }

        [Display(Name = "Label_VaiTro")]
        public string VaiTro { get; set; } = null!;

        [Display(Name = "Label_NgayTao")]
        public DateTime NgayTao { get; set; }
    }
}
