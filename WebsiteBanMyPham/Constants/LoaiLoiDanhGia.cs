namespace WebsiteBanMyPham.Constants
{
    public enum LoaiLoiDanhGia
    {
        KhongCoLoi = 0,
        KhongTimThay = 1,       // 404: Chi tiết đơn hàng không tồn tại hoặc không thuộc khách hàng
        DuLieuKhongHopLe = 2,   // 400: Số sao không hợp lệ (ngoài 1-5), nội dung vượt quá 1000 ký tự
        XungDot = 3             // 409: Đơn hàng chưa HoanThanh hoặc dòng sản phẩm đã được đánh giá trước đó
    }
}

