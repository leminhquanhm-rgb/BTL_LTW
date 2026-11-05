using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class KhieuNai
{
    public int MaKN { get; set; }

    public int MaTK { get; set; }

    public int? MaDH { get; set; }

    public int? MaSP { get; set; }

    public string NoiDung { get; set; } = null!;

    public DateTime NgayGui { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? PhanHoi { get; set; }

    public DateTime? NgayPhanHoi { get; set; }

    public int? MaTKXuLy { get; set; }

    public virtual DonHang? MaDHNavigation { get; set; }

    public virtual SanPham? MaSPNavigation { get; set; }

    public virtual TaiKhoan MaTKNavigation { get; set; } = null!;

    public virtual TaiKhoan? MaTKXuLyNavigation { get; set; }
}
