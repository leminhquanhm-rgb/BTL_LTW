using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IDanhGiaAdminService
    {
        Task<PaginatedList<DanhGiaAdminListItemViewModel>> LayDanhSachAsync(DanhGiaAdminFilterViewModel filter);
        Task<DanhGiaAdminStatusCountViewModel> LaySoLuongTheoTrangThaiAsync(byte? soSao, string? keyword);
        Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int id, string trangThaiCu, string trangThaiMoi);
    }
}

