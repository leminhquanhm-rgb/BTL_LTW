using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IDatHangService
    {
        /// <summary>
        /// Chuẩn bị dữ liệu hiển thị trang đặt hàng cho khách (tính giá, tồn kho, người nhận, phương thức thanh toán, token chống trùng).
        /// </summary>
        Task<(bool HopLe, string? ThongBaoLoi, DatHangViewModel? ViewModel)> ChuanBiDatHangAsync(
            int maTK, List<int>? maGH, int? maSP, int? soLuong, string? codeKM);

        /// <summary>
        /// Kiểm tra tính hợp lệ của nguồn Mua ngay và tính tổng tiền hàng phục vụ API kiểm tra khuyến mãi (Adjustment 5).
        /// </summary>
        Task<(bool HopLe, int StatusCode, string? ErrorTitle, string? ErrorDetail, decimal TongTienHang)> KiemTraNguonMuaNgayChoKhuyenMaiAsync(
            int maSP, int soLuong);

        /// <summary>
        /// Kiểm tra phương thức thanh toán có hợp lệ và được phép sử dụng hay không, trả kèm tên phương thức (TenPT).
        /// </summary>
        Task<(bool HopLe, string? ErrorMessage, string? TenPT)> KiemTraPhuongThucThanhToanAsync(int maPT);
    }
}

