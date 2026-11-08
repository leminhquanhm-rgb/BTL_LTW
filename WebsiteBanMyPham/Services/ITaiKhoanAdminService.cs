using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface ITaiKhoanAdminService
    {
        Task<PaginatedList<KhachHangAdminListItemViewModel>> LayDanhSachKhachHangAsync(KhachHangAdminFilterViewModel filter);
        Task<KhachHangAdminDetailViewModel?> LayChiTietKhachHangAsync(int maTK);
        Task<(bool Success, string? ErrorMessage)> DatTrangThaiKhachHangAsync(int maTK, bool trangThaiMoi);
    }
}

