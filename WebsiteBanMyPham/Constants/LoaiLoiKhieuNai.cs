namespace WebsiteBanMyPham.Constants
{
    public enum LoaiLoiKhieuNai
    {
        KhongCoLoi = 0,
        KhongTimThay = 1,          // 404: Đơn hàng hoặc khiếu nại không tìm thấy / không thuộc khách hàng
        DuLieuKhongHopLe = 2,      // 400: Chưa chọn đơn, sản phẩm không thuộc đơn, nội dung ngoài 10-1000 ký tự
        XungDotTrangThai = 3,      // 409 hoặc 400: Đơn hàng chưa ở trạng thái HoanThanh
        QuaHanKhieuNai = 4,        // 400: Đã quá hạn 10 ngày kể từ ngày giao hàng thành công
        GioiHanSoLuong = 5         // 400 hoặc 409: Đã đạt tối đa 10 khiếu nại đang mở
    }
}

