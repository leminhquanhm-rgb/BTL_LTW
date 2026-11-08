using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IBannerAdminService
    {
        Task<List<BannerAdminListItemViewModel>> LayDanhSachAsync();
        Task<BannerAdminFormViewModel?> LayDeSuaAsync(int maBanner);
        Task<(bool Success, string? ErrorMessage, int? MaBanner)> ThemBannerAsync(BannerAdminFormViewModel model);
        Task<(bool Success, string? ErrorMessage)> CapNhatBannerAsync(int maBanner, BannerAdminFormViewModel model);
        Task<(bool Success, string? ErrorMessage)> XoaBannerAsync(int maBanner);
        Task<(bool Success, string? ErrorMessage)> DatTrangThaiBannerAsync(int maBanner, bool trangThaiMoi);
        Task<(bool Success, string? ErrorMessage)> CapNhatThuTuAsync(int maBanner, int thuTuMoi);
    }
}

