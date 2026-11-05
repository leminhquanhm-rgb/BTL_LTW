using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class VW_DanhGia
{
    public int MaDG { get; set; }

    public int MaCTDH { get; set; }

    public int MaTK { get; set; }

    public int MaSP { get; set; }

    public byte SoSao { get; set; }

    public string? NoiDung { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }
}
