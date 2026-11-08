using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IDiaChiService
    {
        Task<List<DiaChiItemViewModel>> LayDanhSachDiaChiAsync(int maTK);
        Task<DiaChiItemViewModel?> LayChiTietAsync(int maDC, int maTK);
        Task<(bool HopLe, string? ThongBaoLoi, int? MaDCMoi)> ThemDiaChiAsync(int maTK, DiaChiFormViewModel model);
        Task<(bool HopLe, string? ThongBaoLoi)> SuaDiaChiAsync(int maTK, int maDC, DiaChiFormViewModel model);
        Task<(bool HopLe, string? ThongBaoLoi)> XoaDiaChiAsync(int maTK, int maDC);
        Task<(bool HopLe, string? ThongBaoLoi)> DatMacDinhAsync(int maTK, int maDC);
        Task<bool> ThemDiaChiTuDonHangAsync(int maTK, string tenNguoiNhan, string soDienThoai, string diaChi);
    }
}

