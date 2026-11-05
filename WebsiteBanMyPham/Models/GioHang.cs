using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class GioHang
{
    public int MaGH { get; set; }

    public int MaTK { get; set; }

    public int MaSP { get; set; }

    public int SoLuong { get; set; }

    public DateTime NgayThem { get; set; }

    public virtual SanPham MaSPNavigation { get; set; } = null!;

    public virtual TaiKhoan MaTKNavigation { get; set; } = null!;
}
