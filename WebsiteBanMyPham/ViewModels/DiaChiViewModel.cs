using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DiaChiItemViewModel
    {
        public int MaDC { get; set; }
        public string TenNguoiNhan { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public bool LaMacDinh { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class DiaChiDanhSachViewModel
    {
        public List<DiaChiItemViewModel> DanhSach { get; set; } = new();
        public int TongSo => DanhSach.Count;
        public int GioiHanToiDa { get; set; }
        public bool DuocThemTiep => TongSo < GioiHanToiDa;
    }

    public class DiaChiFormViewModel
    {
        public int? MaDC { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenNguoiNhan")]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Validation_Phone")]
        [StringLength(15, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_SDTNguoiNhan")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(255, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_DiaChiGiao")]
        public string DiaChi { get; set; } = string.Empty;

        [Display(Name = "Address_CheckboxSetDefault")]
        public bool LaMacDinh { get; set; }

        public bool LaDiaChiDauTien { get; set; }
        public bool DangLaMacDinh { get; set; }
    }
}

