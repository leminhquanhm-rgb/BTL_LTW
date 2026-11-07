using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IKhieuNaiService
    {
        /// <summary>
        /// Tạo khiếu nại mới theo quy tắc: MaDH bắt buộc, đơn phải HoanThanh trong vòng 10 ngày từ NgayGiaoThanhCong,
        /// MaSP tùy chọn nhưng nếu có phải thuộc đơn, nội dung 10-1000 ký tự, tối đa 10 khiếu nại đang mở.
        /// </summary>
        Task<(bool Success, string? ErrorMessage, LoaiLoiKhieuNai LoaiLoi, int? MaKN)> TaoKhieuNaiAsync(
            int maTK, int maDH, int? maSP, string noiDung);

        /// <summary>
        /// Lấy danh sách khiếu nại của chính khách hàng có phân trang.
        /// </summary>
        Task<KhieuNaiListViewModel> LayDanhSachAsync(int maTK, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// Lấy chi tiết khiếu nại của khách hàng (tuyệt đối không kèm thông tin Admin xử lý).
        /// </summary>
        Task<KhieuNaiChiTietViewModel?> LayChiTietAsync(int maTK, int maKN);

        /// <summary>
        /// Lấy danh sách các đơn hàng đủ điều kiện khiếu nại (HoanThanh, trong vòng 10 ngày từ NgayGiaoThanhCong)
        /// kèm các sản phẩm trong từng đơn để đổ vào form khiếu nại.
        /// </summary>
        Task<List<DonHangSanPhamOptionViewModel>> LayDonHangVaSanPhamCuaKhachAsync(int maTK);
    }
}

