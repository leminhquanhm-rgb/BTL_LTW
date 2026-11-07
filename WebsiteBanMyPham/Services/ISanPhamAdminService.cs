using Microsoft.AspNetCore.Http;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface ISanPhamAdminService
    {
        // 1. Quản lý Danh sách & Chi tiết Sản phẩm
        Task<PaginatedList<SanPhamAdminListItemViewModel>> LayDanhSachAdminAsync(SanPhamAdminFilterViewModel filter);
        Task<SanPhamAdminDetailViewModel?> LayChiTietAdminAsync(int maSP);
        Task<SanPhamCreateFormDropdownsViewModel> LayDuLieuFormThemAsync();
        Task<SanPhamEditFormDropdownsViewModel> LayDuLieuFormSuaAsync(int maSP);

        // 2. Thao tác CRUD Sản phẩm
        Task<(bool Success, string? ErrorMessage, int MaSP)> TaoSanPhamAsync(SanPhamCreateViewModel model, List<IFormFile>? imageFiles);
        Task<(bool Success, string? ErrorMessage)> CapNhatSanPhamAsync(int maSP, SanPhamEditViewModel model);
        Task<(bool Success, string? ErrorMessage)> ChuyenTrangThaiKinhDoanhAsync(int maSP);

        // 3. Quản lý Hình ảnh (AnhSanPham)
        Task<(bool Success, string? ErrorMessage, AnhSanPhamItemViewModel? AnhMoi)> ThemAnhAsync(int maSP, IFormFile file);
        Task<(bool Success, string? ErrorMessage)> DatAnhChinhAsync(int maSP, int maAnh);
        Task<(bool Success, string? ErrorMessage)> XoaAnhAsync(int maSP, int maAnh);

        // 4. Quản lý Tồn kho
        Task<PaginatedList<TonKhoListItemViewModel>> LayDanhSachTonKhoAsync(TonKhoFilterViewModel filter);
        Task<(bool Success, string? ErrorMessage, int SoLuongTonMoi)> DieuChinhTonKhoAsync(int maSP, int delta);
        Task<(bool Success, string? ErrorMessage)> CapNhatNguongCanhBaoAsync(int maSP, int nguongCanhBao);
    }
}

