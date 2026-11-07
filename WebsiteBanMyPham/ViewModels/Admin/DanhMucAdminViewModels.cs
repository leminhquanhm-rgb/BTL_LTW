using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class DanhMucFormViewModel
    {
        public int MaDM { get; set; }

        [Display(Name = "Admin_Category_Name")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(100, ErrorMessage = "Validation_MaxLength")]
        public string TenDM { get; set; } = string.Empty;

        [Display(Name = "Admin_Category_Desc")]
        [MaxLength(500, ErrorMessage = "Validation_MaxLength")]
        public string? MoTa { get; set; }

        [Display(Name = "Admin_Category_Status")]
        public bool TrangThai { get; set; } = true;

        public int SoLuongSanPham { get; set; }
    }

    public class DanhMucListViewModel
    {
        public List<DanhMucListItemViewModel> Items { get; set; } = new();
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class DanhMucListItemViewModel
    {
        public int MaDM { get; set; }
        public string TenDM { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoLuongSanPham { get; set; }
        public int SoSanPhamDangBan { get; set; }
    }
}

