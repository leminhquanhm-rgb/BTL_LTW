using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class TaiKhoan
{
    public int MaTK { get; set; }

    public string Email { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string VaiTro { get; set; } = null!;

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<DatLaiMatKhau> DatLaiMatKhau { get; set; } = new List<DatLaiMatKhau>();

    public virtual ICollection<DonHang> DonHang { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHang { get; set; } = new List<GioHang>();

    public virtual ICollection<KhieuNai> KhieuNaiMaTKNavigation { get; set; } = new List<KhieuNai>();

    public virtual ICollection<KhieuNai> KhieuNaiMaTKXuLyNavigation { get; set; } = new List<KhieuNai>();

    public virtual ICollection<LichSuDonHang> LichSuDonHang { get; set; } = new List<LichSuDonHang>();
}
