using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface ISanPhamService
    {
        /// <summary>
        /// Tìm kiếm và lọc sản phẩm có phân trang (Chỉ lấy sản phẩm TrangThai = true)
        /// </summary>
        Task<PaginatedList<SanPhamItemViewModel>> TimKiemAsync(SanPhamFilterViewModel filter);

        /// <summary>
        /// Lấy chi tiết sản phẩm (bao gồm cả sản phẩm ngừng kinh doanh)
        /// Kèm 10 đánh giá DaDuyet mới nhất và tối đa 4 sản phẩm cùng danh mục đang kinh doanh
        /// </summary>
        Task<SanPhamChiTietViewModel?> LayChiTietAsync(int maSp);

        /// <summary>
        /// Lấy dữ liệu tổng hợp cho Trang chủ: 8 sản phẩm mới, 8 sản phẩm bán chạy (không trùng), banner và khuyến mãi
        /// </summary>
        Task<TrangChuViewModel> LayDanhSachTrangChuAsync();

        /// <summary>
        /// Lấy danh sách Banner đang hoạt động (TrangThai = true, sắp theo ThuTu)
        /// </summary>
        Task<List<BannerItemViewModel>> LayDanhSachBannerAsync();

        /// <summary>
        /// Lấy danh mục đang hoạt động (TrangThai = true, sắp theo TenDM)
        /// </summary>
        Task<List<DanhMucItemViewModel>> LayDanhSachDanhMucAsync();

        /// <summary>
        /// Lấy thương hiệu đang hoạt động (TrangThai = true, sắp theo TenTH)
        /// </summary>
        Task<List<ThuongHieuItemViewModel>> LayDanhSachThuongHieuAsync();
    }
}

