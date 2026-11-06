using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DatHangRequestViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        [Display(Name = "Label_NguonDat")]
        public string NguonDat { get; set; } = "TuGio"; // TuGio hoặc MuaNgay

        // Khi đặt từ giỏ hàng: danh sách mã dòng giỏ được chọn
        public List<int> DanhSachMaGH { get; set; } = new();

        // Khi mua ngay: thông tin sản phẩm và số lượng
        public int? MaSPMuaNgay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Validation_RangeMin")]
        [Display(Name = "Label_SoLuongMuaNgay")]
        public int SoLuongMuaNgay { get; set; } = 1;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenNguoiNhan")]
        public string TenNguoiNhan { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Validation_Phone")]
        [StringLength(15, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_SDTNguoiNhan")]
        public string SdtNguoiNhan { get; set; } = null!;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(255, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_DiaChiGiao")]
        public string DiaChiGiao { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_GhiChu")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [Display(Name = "Label_PhuongThucThanhToan")]
        public int MaPT { get; set; }

        [StringLength(30, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MaKM")]
        public string? CodeKM { get; set; }
    }
}
