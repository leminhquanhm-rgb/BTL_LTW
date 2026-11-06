using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(72, MinimumLength = 6, ErrorMessage = "Validation_StringLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Label_MatKhauHienTai")]
        public string MatKhauHienTai { get; set; } = null!;

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
