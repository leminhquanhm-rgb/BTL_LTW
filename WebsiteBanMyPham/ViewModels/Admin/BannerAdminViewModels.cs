using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebsiteBanMyPham.ViewModels.Admin
{
    public class BannerAdminListItemViewModel
    {
        public int MaBanner { get; set; }
        public string? TieuDe { get; set; }
        public string DuongDanAnh { get; set; } = string.Empty;
        public string? LienKet { get; set; }
        public int ThuTu { get; set; }
        public bool TrangThai { get; set; }
    }

    public class BannerAdminFormViewModel
    {
        public int MaBanner { get; set; }

        [MaxLength(150, ErrorMessage = "Validation_MaxLength")]
        public string? TieuDe { get; set; }

        public string? DuongDanAnh { get; set; }

        public IFormFile? FileAnh { get; set; }

        [MaxLength(255, ErrorMessage = "Validation_MaxLength")]
        public string? LienKet { get; set; }

        [Range(0, 99999, ErrorMessage = "Validation_Range")]
        public int ThuTu { get; set; } = 0;

        public bool TrangThai { get; set; } = true;
    }
}

