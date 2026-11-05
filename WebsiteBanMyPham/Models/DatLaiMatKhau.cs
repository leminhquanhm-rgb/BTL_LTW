using System;
using System.Collections.Generic;

namespace WebsiteBanMyPham.Models;

public partial class DatLaiMatKhau
{
    public int MaDL { get; set; }

    public int MaTK { get; set; }

    public string OtpHash { get; set; } = null!;

    public DateTime HetHan { get; set; }

    public bool DaDung { get; set; }

    public int SoLanSai { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual TaiKhoan MaTKNavigation { get; set; } = null!;
}
