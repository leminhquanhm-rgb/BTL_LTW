using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Services
{
    public interface IThuongHieuService
    {
        Task<PaginatedResponseDto<ThuongHieuAdminDto>> LayDanhSachAdminAsync(string? keyword, bool? trangThai, int pageIndex, int pageSize);
        Task<ThuongHieuAdminDto?> LayChiTietAdminAsync(int maTH);
        Task<(LoaiLoiThuongHieu LoaiLoi, int MaTH, ThuongHieuAdminDto? Dto)> TaoAsync(string tenTH, string? moTa, bool trangThai, string? logo = null);
        Task<(LoaiLoiThuongHieu LoaiLoi, ThuongHieuAdminDto? Dto)> CapNhatAsync(int maTH, string tenTH, string? moTa, bool trangThai, string? logo = null, bool capNhatLogo = false);
        Task<(LoaiLoiThuongHieu LoaiLoi, int SoLuongSP)> XoaAsync(int maTH);
        Task<int> LaySoSanPhamCuaThuongHieuAsync(int maTH);
    }
}

