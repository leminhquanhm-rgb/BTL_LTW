using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Services
{
    public interface IDanhMucService
    {
        Task<PaginatedResponseDto<DanhMucAdminDto>> LayDanhSachAdminAsync(string? keyword, bool? trangThai, int pageIndex, int pageSize);
        Task<DanhMucAdminDto?> LayChiTietAdminAsync(int maDM);
        Task<(LoaiLoiDanhMuc LoaiLoi, int MaDM, DanhMucAdminDto? Dto)> TaoAsync(string tenDM, string? moTa, bool trangThai);
        Task<(LoaiLoiDanhMuc LoaiLoi, DanhMucAdminDto? Dto)> CapNhatAsync(int maDM, string tenDM, string? moTa, bool trangThai);
        Task<(LoaiLoiDanhMuc LoaiLoi, int SoLuongSP)> XoaAsync(int maDM);
    }
}

