using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class ThuongHieuFormViewModel
    {
        public int MaTH { get; set; }

        [Display(Name = "Admin_Brand_Name")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(100, ErrorMessage = "Validation_MaxLength")]
        public string TenTH { get; set; } = string.Empty;

        [Display(Name = "Admin_Brand_Desc")]
        [MaxLength(500, ErrorMessage = "Validation_MaxLength")]
        public string? MoTa { get; set; }

        public string? LogoHienTai { get; set; }

        [Display(Name = "Admin_Brand_Logo")]
        public IFormFile? LogoFile { get; set; }

        public bool XoaLogo { get; set; }

        [Display(Name = "Admin_Brand_Status")]
        public bool TrangThai { get; set; } = true;

        public int SoLuongSanPham { get; set; }
    }

    public class ThuongHieuListViewModel
    {
        public List<ThuongHieuListItemViewModel> Items { get; set; } = new();
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class ThuongHieuListItemViewModel
    {
        public int MaTH { get; set; }
        public string TenTH { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoLuongSanPham { get; set; }
        public int SoSanPhamDangBan { get; set; }
    }
}

