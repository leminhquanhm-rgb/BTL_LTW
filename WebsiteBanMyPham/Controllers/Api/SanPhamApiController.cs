using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/san-pham")]
    [AllowAnonymous]
    public class SanPhamApiController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SanPhamApiController(
            ISanPhamService sanPhamService,
            IStringLocalizer<SharedResource> localizer)
        {
            _sanPhamService = sanPhamService;
            _localizer = localizer;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm đang kinh doanh có tìm kiếm, lọc và phân trang
        /// </summary>
        /// <param name="tuKhoa">Từ khóa tìm kiếm theo tên hoặc thương hiệu (tối đa 100 ký tự)</param>
        /// <param name="danhMuc">Mã danh mục</param>
        /// <param name="thuongHieu">Mã thương hiệu</param>
        /// <param name="giaTu">Giá tối thiểu (VNĐ)</param>
        /// <param name="giaDen">Giá tối đa (VNĐ)</param>
        /// <param name="mucGia">Khoảng giá quy ước: under300, 300to500, over500</param>
        /// <param name="khoangGia">Bí danh khoảng giá (tương đương mucGia): under300, 300to500, over500</param>
        /// <param name="sapXep">Tiêu chí sắp xếp: moiNhat, giaTang, giaGiam, banChay</param>
        /// <param name="pageIndex">Trang hiện tại (>= 1, mặc định 1)</param>
        /// <param name="pageSize">Số sản phẩm mỗi trang (1 - 50, mặc định 12)</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<SanPhamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDanhSach(
            [FromQuery] string? tuKhoa = null,
            [FromQuery] int? danhMuc = null,
            [FromQuery] int? thuongHieu = null,
            [FromQuery] decimal? giaTu = null,
            [FromQuery] decimal? giaDen = null,
            [FromQuery] string? mucGia = null,
            [FromQuery] string? khoangGia = null,
            [FromQuery] string? sapXep = "moiNhat",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            if (giaTu.HasValue && giaDen.HasValue && giaTu.Value > giaDen.Value)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPriceRangeTitle"].Value,
                    Detail = _localizer["Api_Error_InvalidPriceRangeDetail"].Value,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            int validPageIndex = pageIndex < 1 ? 1 : pageIndex;
            int validPageSize = Math.Clamp(pageSize, 1, 50);

            var filter = new SanPhamFilterViewModel
            {
                TuKhoa = tuKhoa,
                DanhMuc = danhMuc,
                ThuongHieu = thuongHieu,
                GiaTu = giaTu,
                GiaDen = giaDen,
                MucGia = mucGia ?? khoangGia,
                SapXep = sapXep,
                Trang = validPageIndex,
                PageSize = validPageSize
            };

            filter.ChuanHoaGia();

            var result = await _sanPhamService.TimKiemAsync(filter);

            var response = new PaginatedResponseDto<SanPhamDto>
            {
                Items = result.Items.Select(p => new SanPhamDto
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    PhanTramGiam = p.PhanTramGiam,
                    AnhChinh = p.AnhChinh,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    DiemSaoTrungBinh = p.DiemSaoTrungBinh,
                    SoDanhGia = p.SoDanhGia,
                    SoLuongTon = p.SoLuongTon,
                    ConHang = p.ConHang
                }).ToList(),
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };

            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một sản phẩm theo ID (kèm đánh giá đã duyệt và sản phẩm liên quan)
        /// </summary>
        /// <param name="id">Mã sản phẩm (MaSP)</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SanPhamChiTietDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            var detail = await _sanPhamService.LayChiTietAsync(id);
            if (detail == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Error_ProductNotFoundDetail", id].Value,
                    Instance = HttpContext.Request.Path
                };
                return NotFound(problemDetails);
            }

            var dto = new SanPhamChiTietDto
            {
                MaSP = detail.MaSP,
                TenSP = detail.TenSP,
                MoTa = detail.MoTa,
                Gia = detail.Gia,
                GiaGoc = detail.GiaGoc,
                PhanTramGiam = detail.PhanTramGiam,
                MaDM = detail.MaDM,
                TenDanhMuc = detail.TenDanhMuc,
                MaTH = detail.MaTH,
                TenThuongHieu = detail.TenThuongHieu,
                SoLuongTon = detail.SoLuongTon,
                DangKinhDoanh = detail.TrangThai,
                ConHang = detail.ConHang,
                DiemSaoTrungBinh = detail.DiemSaoTrungBinh,
                SoDanhGia = detail.SoDanhGia,
                DanhSachAnh = detail.DanhSachAnh,
                DanhGiaMoiNhat = detail.DanhGiaMoiNhat,
                SanPhamLienQuan = detail.SanPhamLienQuan.Select(p => new SanPhamDto
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    PhanTramGiam = p.PhanTramGiam,
                    AnhChinh = p.AnhChinh,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    DiemSaoTrungBinh = p.DiemSaoTrungBinh,
                    SoDanhGia = p.SoDanhGia,
                    SoLuongTon = p.SoLuongTon,
                    ConHang = p.ConHang
                }).ToList()
            };

            return Ok(dto);
        }
    }
}

