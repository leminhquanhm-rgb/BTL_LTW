using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class LichSuDonHang
{
    public int MaLS { get; set; }

    public int MaDH { get; set; }

    public string? TrangThaiCu { get; set; }

    public string TrangThaiMoi { get; set; } = null!;

    public int? MaTKThucHien { get; set; }

    public DateTime NgayDoi { get; set; }

    public virtual DonHang MaDHNavigation { get; set; } = null!;

    public virtual TaiKhoan? MaTKThucHienNavigation { get; set; }
}
