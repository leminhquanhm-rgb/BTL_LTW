using System.ComponentModel.DataAnnotations;

namespace WebsiteBanMyPham.ViewModels
{
    public class DatHangItemViewModel
    {
        public int MaSP { get; set; }
        public int? MaGH { get; set; }
        public string TenSP { get; set; } = null!;
        public string? DuongDanAnh { get; set; }
        public decimal GiaHienTai { get; set; }
        public decimal? GiaGoc { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => GiaHienTai * SoLuong;
        public int SoLuongTon { get; set; }
        public bool TrangThaiSP { get; set; }
        public bool HopLeDeDat { get; set; }
        public string? ThongBaoTrangThai { get; set; }
    }

    public class PhuongThucThanhToanViewModel
    {
        public int MaPT { get; set; }
        public string TenPT { get; set; } = null!;
        public string ResourceKey { get; set; } = null!;
        public string? MoTa { get; set; }
        public bool IsOnline { get; set; }
        public bool IsSelected { get; set; }
    }

    public class DatHangViewModel
    {
        public string NguonDat { get; set; } = "TuGio";
        public List<int> DanhSachMaGH { get; set; } = new();
        public int? MaSPMuaNgay { get; set; }
        public int SoLuongMuaNgay { get; set; } = 1;

        public List<DatHangItemViewModel> Items { get; set; } = new();
        public decimal TongTienHang { get; set; }
        public string? CodeKM { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongThanhToan { get; set; }

        public bool HopLeDeDatHang => Items.Count > 0 && Items.All(i => i.HopLeDeDat);
        public string MaChongDatTrung { get; set; } = string.Empty;

        public List<PhuongThucThanhToanViewModel> DanhSachPhuongThuc { get; set; } = new();
        public bool ChoPhepMoPhong { get; set; }

        // Thông tin người nhận điền sẵn
        public string TenNguoiNhan { get; set; } = string.Empty;
        public string SdtNguoiNhan { get; set; } = string.Empty;
        public string DiaChiGiao { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public int MaPTChon { get; set; }

        // Sổ địa chỉ nhận hàng
        public List<DiaChiItemViewModel> DanhSachDiaChi { get; set; } = new();
        public int? MaDCChon { get; set; }
        public bool CoTheLuuSoDiaChi { get; set; }
        public bool LuuVaoSoDiaChi { get; set; }
        public bool CoDiaChiLuu => DanhSachDiaChi.Count > 0;
    }

    public class DatHangFormViewModel
    {
        [Required(ErrorMessage = "Validation_Required")]
        public string NguonDat { get; set; } = "TuGio";

        public List<int>? DanhSachMaGH { get; set; }

        public int? MaSPMuaNgay { get; set; }

        [Range(1, 99, ErrorMessage = "Validation_RangeMin")]
        public int SoLuongMuaNgay { get; set; } = 1;

        public int? MaDC { get; set; }
        public bool LuuVaoSoDiaChi { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(100, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_TenNguoiNhan")]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        [RegularExpression(@"^0[0-9]{9,14}$", ErrorMessage = "Validation_Phone")]
        [StringLength(15, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_SDTNguoiNhan")]
        public string SdtNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Validation_Required")]
        [StringLength(255, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_DiaChiGiao")]
        public string DiaChiGiao { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_GhiChu")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        [Display(Name = "Label_PhuongThucThanhToan")]
        public int MaPT { get; set; }

        [StringLength(30, ErrorMessage = "Validation_MaxLength")]
        [Display(Name = "Label_MaKM")]
        public string? CodeKM { get; set; }

        [Required(ErrorMessage = "Validation_Required")]
        public string MaChongDatTrung { get; set; } = null!;
    }
}

