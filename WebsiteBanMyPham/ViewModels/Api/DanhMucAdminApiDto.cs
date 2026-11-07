using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    /// <summary>
    /// DTO thông tin danh mục phục vụ trang quản trị
    /// </summary>
    public class DanhMucAdminDto
    {
        public int MaDM { get; set; }
        public string TenDM { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoLuongSanPham { get; set; }
    }

    /// <summary>
    /// DTO yêu cầu thêm mới danh mục
    /// </summary>
    public class TaoDanhMucRequestDto
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenDM")]
        public string TenDM { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MoTa")]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    /// <summary>
    /// DTO yêu cầu cập nhật danh mục
    /// </summary>
    public class CapNhatDanhMucRequestDto
    {
        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenDM")]
        public string TenDM { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MoTa")]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }
    }
}

