using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels.Api
{
    public class CapNhatGioHangRequestDto
    {
        [Required]
        [Range(1, 99, ErrorMessage = "Số lượng phải từ 1 đến 99.")]
        public int SoLuong { get; set; }
    }
}

