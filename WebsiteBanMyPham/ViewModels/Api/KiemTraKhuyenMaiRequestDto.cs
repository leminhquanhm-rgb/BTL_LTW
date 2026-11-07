using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    public class KiemTraKhuyenMaiRequestDto
    {
        [Required(ErrorMessage = "Validation_Required")]
        [MaxLength(30, ErrorMessage = "Validation_MaxLength")]
        public string Code { get; set; } = null!;

        /// <summary>
        /// Nguồn từ giỏ hàng: danh sách mã dòng giỏ được chọn
        /// </summary>
        public List<int>? MaGHDaChon { get; set; }

        /// <summary>
        /// Nguồn Mua ngay: mã sản phẩm
        /// </summary>
        public int? MaSP { get; set; }

        /// <summary>
        /// Nguồn Mua ngay: số lượng đặt mua (1 đến 99)
        /// </summary>
        [Range(1, 99, ErrorMessage = "Validation_RangeMin")]
        public int? SoLuong { get; set; }
    }
}
