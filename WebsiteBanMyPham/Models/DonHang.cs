using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class DonHang
{
    public int MaDH { get; set; }

    public int MaTK { get; set; }

    public DateTime NgayDat { get; set; }

    public string TenNguoiNhan { get; set; } = null!;

    public string SdtNguoiNhan { get; set; } = null!;

    public string DiaChiGiao { get; set; } = null!;

    public string? GhiChu { get; set; }

    public decimal TongTienHang { get; set; }

    public int? MaKM { get; set; }

    public string? CodeKM { get; set; }

    public decimal TienGiam { get; set; }

    public decimal TongThanhToan { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime? NgayVanDon { get; set; }

    public DateTime? NgayGiaoThanhCong { get; set; }

    public string? MaVanDon { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<KhieuNai> KhieuNai { get; set; } = new List<KhieuNai>();

    public virtual ICollection<LichSuDonHang> LichSuDonHang { get; set; } = new List<LichSuDonHang>();

    public virtual KhuyenMai? MaKMNavigation { get; set; }

    public virtual TaiKhoan MaTKNavigation { get; set; } = null!;

    public virtual ThanhToan? ThanhToan { get; set; }
}
