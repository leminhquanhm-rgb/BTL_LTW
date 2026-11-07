using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IGioHangService
    {
        Task<GioHangViewModel> LayGioHangAsync(int maTK);
        Task<GioHangItemViewModel?> LayDongGioHangAsync(int maTK, int maGH);
        Task<(bool Success, string? ErrorMessage, LoaiLoiGioHang LoaiLoi, int MaGH, int SoLuongDong, bool DaTonTaiTruocDo)> ThemVaoGioAsync(int maTK, int maSP, int soLuong);
        Task<(bool Success, string? ErrorMessage, LoaiLoiGioHang LoaiLoi)> CapNhatSoLuongAsync(int maTK, int maGH, int soLuongMoi);
        Task<(bool Success, string? ErrorMessage, LoaiLoiGioHang LoaiLoi)> XoaKhoiGioAsync(int maTK, int maGH);
        Task<int> DemSoLuongAsync(int maTK);
    }
}

