using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    public class ThemGioHangRequestDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Mã sản phẩm không hợp lệ.")]
        public int MaSP { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Số lượng phải từ 1 đến 99.")]
        public int SoLuong { get; set; } = 1;
    }
}

