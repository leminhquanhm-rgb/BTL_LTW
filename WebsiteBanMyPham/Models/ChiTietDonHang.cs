using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class ChiTietDonHang
{
    public int MaCTDH { get; set; }

    public int MaDH { get; set; }

    public int MaSP { get; set; }

    public string TenSP { get; set; } = null!;

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual DanhGia? DanhGia { get; set; }

    public virtual DonHang MaDHNavigation { get; set; } = null!;

    public virtual SanPham MaSPNavigation { get; set; } = null!;
}
