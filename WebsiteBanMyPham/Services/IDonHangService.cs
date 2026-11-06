using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IDonHangService
    {
        // Tạo đơn hàng (Transaction tường minh)
        Task<(bool Success, string? ErrorMessage, int? MaDH)> TaoDonAsync(int maTK, DatHangRequestViewModel request);

        // Hủy đơn hàng (Khách hàng)
        Task<(bool Success, string? ErrorMessage)> HuyDonAsync(int maTK, int maDH);

        /// <summary>
        /// Cập nhật trạng thái đơn hàng theo luồng hợp lệ. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int maTKAdmin, int maDH, string trangThaiMoi, string? maVanDon = null);

        /// <summary>
        /// Xác nhận hoàn tiền cho đơn hàng. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> XacNhanHoanTienAsync(int maTKAdmin, int maDH);

        // Thanh toán Online mô phỏng
        Task<(bool Success, string? ErrorMessage)> GhiNhanThanhToanOnlineMoPhongAsync(int maTK, int maDH, bool thanhCong);

        // Truy vấn đơn hàng Khách hàng
        Task<DonHangKhachListViewModel> LayDonHangCuaKhachAsync(int maTK, string? trangThai, int pageIndex = 1, int pageSize = 10);
        Task<DonHangKhachChiTietViewModel?> LayChiTietDonHangKhachAsync(int maTK, int maDH);

        /// <summary>
        /// Lấy danh sách đơn hàng cho Admin. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        Task<DonHangAdminListViewModel> LayDonHangAdminAsync(string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// Lấy chi tiết đơn hàng cho Admin. Chỉ gọi từ khu vực Admin có [Authorize(Roles = "QuanTriVien")].
        /// </summary>
        Task<DonHangAdminChiTietViewModel?> LayChiTietDonHangAdminAsync(int maDH);
    }
}
