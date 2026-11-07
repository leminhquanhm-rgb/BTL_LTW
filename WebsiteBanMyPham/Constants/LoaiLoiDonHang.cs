namespace WebsiteBanMyPham.Constants
{
    public enum LoaiLoiDonHang
    {
        KhongCoLoi = 0,
        KhongTimThay = 1,      // 404: Đơn hàng không tồn tại hoặc không thuộc khách hàng
        XungDotTrangThai = 2,  // 409: Đơn hàng không còn ở trạng thái ChoXacNhan
        LoiHeThong = 3         // 500: Lỗi hệ thống trong transaction hủy đơn
    }
}

