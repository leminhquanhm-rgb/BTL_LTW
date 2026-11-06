using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class QuenMatKhauViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [EmailAddress(ErrorMessage = "Validation_Email")]
        [StringLength(150, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_Email")]
        public string Email { get; set; } = null!;
    }
}
