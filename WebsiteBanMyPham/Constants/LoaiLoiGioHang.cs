namespace WebsiteBanMyPham.Constants
{
    public enum LoaiLoiGioHang
    {
        KhongCoLoi = 0,
        KhongTimThay = 1,              // 404: Dòng giỏ hoặc sản phẩm không tồn tại / không thuộc quyền sở hữu
        DuLieuKhongHopLe = 2,          // 400: Số lượng không hợp lệ, dữ liệu sai định dạng
        XungDotTonKhoHoacNgungBan = 3  // 409: Sản phẩm hết hàng, ngừng kinh doanh hoặc số lượng yêu cầu vượt quá tồn kho
    }
}

