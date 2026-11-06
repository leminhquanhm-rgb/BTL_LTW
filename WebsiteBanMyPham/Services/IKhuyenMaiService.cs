using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IKhuyenMaiService
    {
        Task<KhuyenMaiKetQuaViewModel> TinhGiamAsync(string? code, decimal tongTienHang);
        Task<List<KhuyenMaiItemViewModel>> LayDanhSachMaKhaDungAsync(decimal tongTienHang);
    }
}

