using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DatHangRequestViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nguồn đặt hàng")]
        public string NguonDat { get; set; } = "TuGio"; // TuGio hoặc MuaNgay

        // Khi đặt từ giỏ hàng: danh sách mã dòng giỏ được chọn
        public List<int> DanhSachMaGH { get; set; } = new();

        // Khi mua ngay: thông tin sản phẩm và số lượng
        public int? MaSPMuaNgay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng mua ngay phải lớn hơn 0")]
        public int SoLuongMuaNgay { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [StringLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên người nhận")]
        public string TenNguoiNhan { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Số điện thoại không đúng định dạng (bắt đầu bằng 0, độ dài 10-15 chữ số)")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Display(Name = "Số điện thoại nhận")]
        public string SdtNguoiNhan { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(255, ErrorMessage = "Địa chỉ giao hàng không được vượt quá 255 ký tự")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiao { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú đơn hàng")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public int MaPT { get; set; }

        [StringLength(30, ErrorMessage = "Mã khuyến mãi không được vượt quá 30 ký tự")]
        [Display(Name = "Mã khuyến mãi")]
        public string? CodeKM { get; set; }
    }
}

