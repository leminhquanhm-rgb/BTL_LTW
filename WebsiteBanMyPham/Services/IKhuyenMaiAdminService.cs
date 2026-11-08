using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IKhuyenMaiAdminService
    {
        Task<PaginatedList<KhuyenMaiAdminListItemViewModel>> LayDanhSachAsync(KhuyenMaiAdminFilterViewModel filter);
        Task<KhuyenMaiAdminFormViewModel?> LayDeSuaAsync(int maKM);
        Task<(bool Success, string? ErrorMessage, int? MaKM)> ThemKhuyenMaiAsync(KhuyenMaiAdminFormViewModel model);
        Task<(bool Success, string? ErrorMessage)> CapNhatKhuyenMaiAsync(int maKM, KhuyenMaiAdminFormViewModel model);
        Task<(bool Success, string? ErrorMessage)> XoaKhuyenMaiAsync(int maKM);
        Task<(bool Success, string? ErrorMessage)> DatTrangThaiKhuyenMaiAsync(int maKM, bool trangThaiMoi);
    }
}

