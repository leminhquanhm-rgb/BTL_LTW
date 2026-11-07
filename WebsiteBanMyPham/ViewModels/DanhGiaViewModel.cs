using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class ThongTinDanhGiaViewModel
    {
        public int MaCTDH { get; set; }
        public int MaDH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
        public bool CoTheDanhGia { get; set; }
        public string? LyDoKhongTheDanhGia { get; set; }
    }

    public class TaoDanhGiaViewModel
    {
        [Required]
        public int MaCTDH { get; set; }

        public int MaDH { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [Range(1, 5, ErrorMessage = "Review_Error_InvalidStar")]
        public byte SoSao { get; set; } = 5;

        [StringLength(1000, ErrorMessage = "Review_Error_ContentTooLong")]
        public string? NoiDung { get; set; }
    }
}

