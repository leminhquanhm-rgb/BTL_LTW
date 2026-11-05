using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class Banner
{
    public int MaBanner { get; set; }

    public string? TieuDe { get; set; }

    public string DuongDanAnh { get; set; } = null!;

    public string? LienKet { get; set; }

    public int ThuTu { get; set; }

    public bool TrangThai { get; set; }
}
