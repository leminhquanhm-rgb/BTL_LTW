using System.ComponentModel.DataAnnotations;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.ViewModels
{
    public class KhieuNaiItemViewModel
    {
        public int MaKN { get; set; }
        public int? MaDH { get; set; }
        public int? MaSP { get; set; }
        public string? TenSP { get; set; }
        public string NoiDung { get; set; } = null!;
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
    }

    public class KhieuNaiListViewModel
    {
        public List<KhieuNaiItemViewModel> Items { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; } = 1;
    }

    public class KhieuNaiChiTietViewModel
    {
        public int MaKN { get; set; }
        public int? MaDH { get; set; }
        public int? MaSP { get; set; }
        public string? TenSP { get; set; }
        public string? DuongDanAnhSP { get; set; }
        public decimal? DonGiaSP { get; set; }
        public string NoiDung { get; set; } = null!;
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? PhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
    }

    public class SanPhamOptionItem
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = null!;
    }

    public class DonHangSanPhamOptionViewModel
    {
        public int MaDH { get; set; }
        public string MaDonHangText { get; set; } = null!;
        public DateTime NgayGiaoThanhCong { get; set; }
        public List<SanPhamOptionItem> SanPhams { get; set; } = new();
    }

    public class TaoKhieuNaiViewModel
    {
        [Required(ErrorMessage = "Complaint_Error_NoOrderSelected")]
        [Display(Name = "Complaint_RelatedOrder")]
        public int MaDH { get; set; }

        [Display(Name = "Complaint_RelatedProduct")]
        public int? MaSP { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(KhieuNaiConstant.DoDaiNoiDungToiDa, MinimumLength = KhieuNaiConstant.DoDaiNoiDungToiThieu, ErrorMessage = "Complaint_Error_ContentLength")]
        [Display(Name = "Complaint_Content")]
        public string NoiDung { get; set; } = string.Empty;

        public List<DonHangSanPhamOptionViewModel> DanhSachDonHang { get; set; } = new();
    }
}

