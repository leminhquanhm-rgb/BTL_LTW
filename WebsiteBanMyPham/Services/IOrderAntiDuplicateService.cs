namespace WebsiteBanMyPham.Services
{
    public enum OrderStatusTokenState
    {
        Issued,
        Processing,
        Completed
    }

    public enum ThuNhanMaResultType
    {
        Accepted,
        AlreadyCompleted,
        InProgress,
        Invalid
    }

    public record ThuNhanMaResult(ThuNhanMaResultType Status, int? MaDH = null);

    public interface IOrderAntiDuplicateService
    {
        /// <summary>
        /// Tạo và phát một mã token chống đặt trùng (trạng thái Issued), giới hạn 15 phút, tối đa 20 mã mỗi tài khoản.
        /// </summary>
        string PhatMa(int maTK);

        /// <summary>
        /// Thao tác nguyên tử thu nhận mã: chuyển trạng thái Issued sang Processing.
        /// </summary>
        ThuNhanMaResult ThuNhanMa(string? token, int maTK);

        /// <summary>
        /// Cập nhật kết quả khi tạo đơn thành công: chuyển sang Completed(MaDH).
        /// </summary>
        void CapNhatKetQua(string? token, int maDH);

        /// <summary>
        /// Hủy/xóa mã khi tạo đơn thất bại để tránh lưu trạng thái dở dang.
        /// </summary>
        void HuyMaKhiLoi(string? token);
    }
}

