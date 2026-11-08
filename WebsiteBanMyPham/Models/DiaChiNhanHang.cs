using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class DiaChiNhanHang
{
    public int MaDC { get; set; }

    public int MaTK { get; set; }

    public string TenNguoiNhan { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public bool LaMacDinh { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual TaiKhoan MaTKNavigation { get; set; } = null!;
}

