using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class SanPhamAdminFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? MaDM { get; set; }
        public int? MaTH { get; set; }
        public bool? TrangThai { get; set; }
        public string? TinhTrangTon { get; set; } // "tatCa", "sapHet", "hetHang"
        public int Trang { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SanPhamAdminListItemViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string? AnhChinh { get; set; }
        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public decimal? GiaGoc { get; set; }
        public int SoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public bool TrangThai { get; set; }
        public bool TrangThaiDanhMuc { get; set; }
        public bool TrangThaiThuongHieu { get; set; }
        public bool IsDangBan { get; set; }
        public string TrangThaiHienThiKey { get; set; } = string.Empty;
        public int SoLuongAnh { get; set; }
    }

    public class AnhSanPhamItemViewModel
    {
        public int MaAnh { get; set; }
        public int MaSP { get; set; }
        public string DuongDan { get; set; } = string.Empty;
        public bool LaAnhChinh { get; set; }
    }

    public class SanPhamAdminDetailViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal Gia { get; set; }
        public decimal? GiaGoc { get; set; }
        public int SoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public bool TrangThai { get; set; }
        public int MaDM { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public bool TrangThaiDanhMuc { get; set; }
        public int MaTH { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public bool TrangThaiThuongHieu { get; set; }
        public bool IsDangBan { get; set; }
        public DateTime NgayTao { get; set; }
        public List<AnhSanPhamItemViewModel> DanhSachAnh { get; set; } = new();
    }

    public class SanPhamCreateViewModel
    {
        [Display(Name = "Admin_Product_Name")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(200, ErrorMessage = "Validation_MaxLength")]
        public string TenSP { get; set; } = string.Empty;

        [Display(Name = "Admin_Product_Description")]
        public string? MoTa { get; set; }

        [Display(Name = "Admin_Product_Price")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(0, 1000000000, ErrorMessage = "Validation_Range")]
        public decimal Gia { get; set; }

        [Display(Name = "Admin_Product_OriginalPrice")]
        [Range(0, 1000000000, ErrorMessage = "Validation_Range")]
        public decimal? GiaGoc { get; set; }

        [Display(Name = "Admin_Product_InitialStock")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(0, 1000000, ErrorMessage = "Validation_Range")]
        public int SoLuongTon { get; set; } = 0;

        [Display(Name = "Admin_Product_WarningThreshold")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(0, 100000, ErrorMessage = "Validation_Range")]
        public int NguongCanhBao { get; set; } = 5;

        [Display(Name = "Admin_Product_Status")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Admin_Product_Category")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Validation_Required")]
        public int MaDM { get; set; }

        [Display(Name = "Admin_Product_Brand")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Validation_Required")]
        public int MaTH { get; set; }

        [Display(Name = "Admin_Product_Images")]
        public List<IFormFile>? AnhFiles { get; set; }
    }

    public class SanPhamEditViewModel
    {
        public int MaSP { get; set; }

        [Display(Name = "Admin_Product_Name")]
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(200, ErrorMessage = "Validation_MaxLength")]
        public string TenSP { get; set; } = string.Empty;

        [Display(Name = "Admin_Product_Description")]
        public string? MoTa { get; set; }

        [Display(Name = "Admin_Product_Price")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(0, 1000000000, ErrorMessage = "Validation_Range")]
        public decimal Gia { get; set; }

        [Display(Name = "Admin_Product_OriginalPrice")]
        [Range(0, 1000000000, ErrorMessage = "Validation_Range")]
        public decimal? GiaGoc { get; set; }

        [Display(Name = "Admin_Product_WarningThreshold")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(0, 100000, ErrorMessage = "Validation_Range")]
        public int NguongCanhBao { get; set; } = 5;

        [Display(Name = "Admin_Product_Status")]
        public bool TrangThai { get; set; }

        [Display(Name = "Admin_Product_Category")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Validation_Required")]
        public int MaDM { get; set; }

        [Display(Name = "Admin_Product_Brand")]
        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Validation_Required")]
        public int MaTH { get; set; }

        // Thông tin chỉ đọc
        public int SoLuongTon { get; set; }
        public DateTime NgayTao { get; set; }
        public List<AnhSanPhamItemViewModel> DanhSachAnh { get; set; } = new();
    }

    public class SanPhamCreateFormDropdownsViewModel
    {
        public List<SelectListItem> DanhMucs { get; set; } = new();
        public List<SelectListItem> ThuongHieus { get; set; } = new();
    }

    public class SanPhamEditFormDropdownsViewModel
    {
        public List<SelectListItem> DanhMucs { get; set; } = new();
        public List<SelectListItem> ThuongHieus { get; set; } = new();
    }
}

