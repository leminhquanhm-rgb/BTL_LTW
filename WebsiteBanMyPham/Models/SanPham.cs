using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class SanPham
{
    public int MaSP { get; set; }

    public string TenSP { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal Gia { get; set; }

    public decimal? GiaGoc { get; set; }

    public int SoLuongTon { get; set; }

    public int NguongCanhBao { get; set; }

    public bool TrangThai { get; set; }

    public int MaDM { get; set; }

    public int MaTH { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<AnhSanPham> AnhSanPham { get; set; } = new List<AnhSanPham>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<GioHang> GioHang { get; set; } = new List<GioHang>();

    public virtual ICollection<KhieuNai> KhieuNai { get; set; } = new List<KhieuNai>();

    public virtual DanhMuc MaDMNavigation { get; set; } = null!;

    public virtual ThuongHieu MaTHNavigation { get; set; } = null!;
}
