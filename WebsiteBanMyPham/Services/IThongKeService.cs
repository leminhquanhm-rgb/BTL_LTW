using WebsiteBanMyPham.ViewModels.Admin;

namespace WebsiteBanMyPham.Services
{
    public interface IThongKeService
    {
        Task<DashboardViewModel> LayDuLieuDashboardAsync(int? nam = null);
        Task<List<DoanhThuThangExcelDto>> LayDoanhThu12ThangAsync(int nam);
        Task<List<DonHangHoanThanhThangExcelDto>> LayDonHoanThanhTheoThangAsync(int nam, int thang);
        Task<List<SanPhamTonKhoExcelDto>> LayDuLieuSanPhamTonKhoExcelAsync();
        Task<List<DonHangExcelDto>> LayDuLieuDonHangExcelAsync(
            string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword);
    }
}

