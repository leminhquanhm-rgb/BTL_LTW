using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class DanhMuc
{
    public int MaDM { get; set; }

    public string TenDM { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<SanPham> SanPham { get; set; } = new List<SanPham>();
}
