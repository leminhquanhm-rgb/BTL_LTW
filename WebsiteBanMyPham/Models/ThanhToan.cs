using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class ThanhToan
{
    public int MaTT { get; set; }

    public int MaDH { get; set; }

    public int MaPT { get; set; }

    public decimal SoTien { get; set; }

    public string TrangThai { get; set; } = null!;

    public string TrangThaiHoanTien { get; set; } = null!;

    public string? MaGiaoDich { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public DateTime? NgayHoanTien { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual DonHang MaDHNavigation { get; set; } = null!;

    public virtual PhuongThucThanhToan MaPTNavigation { get; set; } = null!;
}
