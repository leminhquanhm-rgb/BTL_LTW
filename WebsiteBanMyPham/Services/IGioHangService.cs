using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IGioHangService
    {
        Task<GioHangViewModel> LayGioHangAsync(int maTK);
        Task<(bool Success, string? ErrorMessage)> ThemVaoGioAsync(int maTK, int maSP, int soLuong);
        Task<(bool Success, string? ErrorMessage)> CapNhatSoLuongAsync(int maTK, int maGH, int soLuongMoi);
        Task<(bool Success, string? ErrorMessage)> XoaKhoiGioAsync(int maTK, int maGH);
        Task<int> DemSoLuongAsync(int maTK);
    }
}

