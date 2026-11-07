using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    /// <summary>
    /// DTO thông tin thương hiệu phục vụ trang quản trị
    /// </summary>
    public class ThuongHieuAdminDto
    {
        public int MaTH { get; set; }
        public string TenTH { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoLuongSanPham { get; set; }
    }

    /// <summary>
    /// DTO yêu cầu thêm mới thương hiệu
    /// </summary>
    public class TaoThuongHieuRequestDto
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenTH")]
        public string TenTH { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MoTa")]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    /// <summary>
    /// DTO yêu cầu cập nhật thương hiệu
    /// </summary>
    public class CapNhatThuongHieuRequestDto
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenTH")]
        public string TenTH { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MoTa")]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }
    }
}

