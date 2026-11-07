using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface IDanhGiaService
    {
        /// <summary>
        /// Lấy thông tin dòng chi tiết đơn hàng để chuẩn bị form đánh giá (tên SP, ảnh, mã đơn, và kiểm tra điều kiện đánh giá).
        /// Trả về null nếu dòng chi tiết không tồn tại hoặc không thuộc quyền sở hữu của maTK.
        /// </summary>
        Task<ThongTinDanhGiaViewModel?> LayThongTinDanhGiaAsync(int maTK, int maCTDH);

        /// <summary>
        /// Tạo đánh giá mới (1-5 sao, nội dung tối đa 1000 ký tự). Trạng thái mới: ChoDuyet.
        /// Bắt DbUpdateException do ràng buộc UNIQUE MaCTDH và chuyển thành lỗi xung đột "đã đánh giá".
        /// </summary>
        Task<(bool Success, string? ErrorMessage, LoaiLoiDanhGia LoaiLoi)> TaoDanhGiaAsync(int maTK, int maCTDH, byte soSao, string? noiDung);
    }
}

