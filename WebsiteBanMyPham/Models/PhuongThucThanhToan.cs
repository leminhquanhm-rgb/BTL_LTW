using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class PhuongThucThanhToan
{
    public int MaPT { get; set; }

    public string TenPT { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<ThanhToan> ThanhToan { get; set; } = new List<ThanhToan>();
}
