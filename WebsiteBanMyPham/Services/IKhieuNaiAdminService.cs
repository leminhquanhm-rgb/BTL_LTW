using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IKhieuNaiAdminService
    {
        Task<PaginatedList<KhieuNaiAdminListItemViewModel>> LayDanhSachAsync(KhieuNaiAdminFilterViewModel filter);
        Task<KhieuNaiAdminStatusCountViewModel> LaySoLuongTheoTrangThaiAsync(string? keyword);
        Task<KhieuNaiAdminDetailViewModel?> LayChiTietAsync(int id);
        Task<(bool Success, string? ErrorMessage)> XuLyKhieuNaiAsync(int id, string trangThaiCu, string trangThaiMoi, string? phanHoi, int maTKAdmin);
    }
}

