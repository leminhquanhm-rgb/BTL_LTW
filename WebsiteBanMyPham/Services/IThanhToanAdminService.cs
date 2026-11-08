using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IThanhToanAdminService
    {
        // 1. Quản lý Giao dịch Thanh toán & Hoàn tiền
        Task<PaginatedList<ThanhToanAdminListItemViewModel>> LayDanhSachGiaoDichAsync(ThanhToanAdminFilterViewModel filter);
        Task<ThanhToanAdminOverviewViewModel> LayTongQuanGiaoDichAsync();

        // 2. Quản lý Phương thức thanh toán
        Task<List<PhuongThucThanhToanAdminViewModel>> LayDanhSachPhuongThucAsync();
        Task<PhuongThucThanhToanAdminViewModel?> LayChiTietPhuongThucAsync(int maPT);
        Task<(bool Success, string? ErrorMessage)> CapNhatPhuongThucAsync(int maPT, string? moTa);
        Task<(bool Success, string? ErrorMessage)> DatTrangThaiPhuongThucAsync(int maPT, bool trangThaiMoi);
    }
}

