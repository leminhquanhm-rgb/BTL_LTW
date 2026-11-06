using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [EmailAddress(ErrorMessage = "Validation_Email")]
        [StringLength(150, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(72, MinimumLength = 6, ErrorMessage = "Validation_StringLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Label_MatKhau")]
        public string MatKhau { get; set; } = null!;

        [Display(Name = "Label_GhiNhoDangNhap")]
        public bool RememberMe { get; set; }
    }
}
