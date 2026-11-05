using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class AnhSanPham
{
    public int MaAnh { get; set; }

    public int MaSP { get; set; }

    public string DuongDan { get; set; } = null!;

    public bool LaAnhChinh { get; set; }

    public virtual SanPham MaSPNavigation { get; set; } = null!;
}
