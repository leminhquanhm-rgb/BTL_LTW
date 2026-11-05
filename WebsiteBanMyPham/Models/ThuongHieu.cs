using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class ThuongHieu
{
    public int MaTH { get; set; }

    public string TenTH { get; set; } = null!;

    public string? Logo { get; set; }

    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<SanPham> SanPham { get; set; } = new List<SanPham>();
}
