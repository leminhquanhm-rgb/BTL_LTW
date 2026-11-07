using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    public class KiemTraKhuyenMaiRequestDto
    {
        [Required(ErrorMessage = "Mã khuyến mãi không được để trống.")]
        [MaxLength(30, ErrorMessage = "Mã khuyến mãi tối đa 30 ký tự.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Danh sách dòng giỏ không được để trống.")]
        public List<int> MaGHDaChon { get; set; } = new();
    }
}

