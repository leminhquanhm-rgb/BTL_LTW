namespace WebsiteBanMyPham.Constants
{
    public static class SessionKeyConstant
    {
        /// <summary>
        /// Khóa Session lưu danh sách mã sản phẩm (List&lt;int&gt;) đã xem gần đây.
        /// </summary>
        public const string SanPhamDaXem = "SanPhamDaXem";

        /// <summary>
        /// Số lượng sản phẩm tối đa được lưu trữ trong danh sách Session.
        /// </summary>
        public const int SoLuongLuuToiDa = 8;

        /// <summary>
        /// Số lượng sản phẩm tối đa được hiển thị trên giao diện người dùng.
        /// </summary>
        public const int SoLuongHienThiToiDa = 6;
    }
}

