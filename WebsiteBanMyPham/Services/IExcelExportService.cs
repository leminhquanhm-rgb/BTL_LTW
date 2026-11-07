namespace WebsiteBanMyPham.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> XuatDoanhThuTheoThangAsync(int nam);
        Task<byte[]> XuatDoanhThuChiTietThangAsync(int nam, int thang);
        Task<byte[]> XuatSanPhamTonKhoAsync();
        Task<byte[]> XuatDanhSachDonHangAsync(string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword);
    }
}

