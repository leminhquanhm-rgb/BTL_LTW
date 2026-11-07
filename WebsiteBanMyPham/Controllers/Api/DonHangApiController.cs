using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    /// <summary>
    /// API Quản lý Đơn hàng cho Khách hàng
    /// </summary>
    [ApiController]
    [Route("api/v1/don-hang")]
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DonHangApiController : ControllerBase
    {
        private readonly IDonHangService _donHangService;
        private readonly ILocalizationHelper _localizationHelper;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DonHangApiController(
            IDonHangService donHangService,
            ILocalizationHelper localizationHelper,
            IStringLocalizer<SharedResource> localizer)
        {
            _donHangService = donHangService;
            _localizationHelper = localizationHelper;
            _localizer = localizer;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của khách hàng (có lọc trạng thái, phân trang).
        /// </summary>
        /// <param name="trangThai">Trạng thái đơn hàng cần lọc</param>
        /// <param name="pageIndex">Trang hiện tại (>= 1, mặc định 1)</param>
        /// <param name="pageSize">Số đơn hàng mỗi trang (1 - 50, mặc định 10)</param>
        [HttpGet]
        [ProducesResponseType(typeof(DonHangKhachListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LayDanhSach(
            [FromQuery] string? trangThai,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = _localizer["Api_Error_UnauthorizedTitle"].Value,
                    Detail = _localizer["Api_Error_UnauthorizedDetail"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            // Xử lý phân trang giống SanPhamApiController (pageIndex < 1 gán 1, pageSize Math.Clamp 1-50)
            if (pageIndex < 1) pageIndex = 1;
            pageSize = Math.Clamp(pageSize, 1, 50);

            var result = await _donHangService.LayDonHangCuaKhachAsync(maTK.Value, trangThai, pageIndex, pageSize);

            var dto = new DonHangKhachListDto
            {
                PageIndex = result.PageIndex,
                PageSize = pageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                Items = result.DonHangs.Select(d => new DonHangKhachItemDto
                {
                    MaDH = d.MaDH,
                    MaDonHangText = _localizationHelper.MaDonHang(d.MaDH),
                    NgayDat = d.NgayDat,
                    TongTienHang = d.TongTienHang,
                    TienGiam = d.TienGiam,
                    TongThanhToan = d.TongThanhToan,
                    TrangThai = d.TrangThai,
                    TenPhuongThuc = d.TenPhuongThuc,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    SoLuongSanPham = d.SoLuongSanPham,
                    SanPham = d.SanPham.Select(s => new ChiTietSanPhamDonHangDto
                    {
                        MaCTDH = s.MaCTDH,
                        MaSP = s.MaSP,
                        TenSP = s.TenSP,
                        DuongDanAnh = s.DuongDanAnh,
                        DonGia = s.DonGia,
                        SoLuong = s.SoLuong,
                        ThanhTien = s.ThanhTien,
                        DaDanhGia = s.DaDanhGia,
                        TrangThaiDanhGia = s.TrangThaiDanhGia
                    }).ToList()
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng của khách hàng (lịch sử không lộ thông tin Admin).
        /// </summary>
        /// <param name="maDH">Mã đơn hàng</param>
        [HttpGet("{maDH:int}")]
        [ProducesResponseType(typeof(DonHangKhachChiTietDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LayChiTiet(int maDH)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = _localizer["Api_Error_UnauthorizedTitle"].Value,
                    Detail = _localizer["Api_Error_UnauthorizedDetail"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var donHang = await _donHangService.LayChiTietDonHangKhachAsync(maTK.Value, maDH);
            if (donHang == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Msg_Order_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var dto = new DonHangKhachChiTietDto
            {
                MaDH = donHang.MaDH,
                MaDonHangText = _localizationHelper.MaDonHang(donHang.MaDH),
                NgayDat = donHang.NgayDat,
                TenNguoiNhan = donHang.TenNguoiNhan,
                SdtNguoiNhan = donHang.SdtNguoiNhan,
                DiaChiGiao = donHang.DiaChiGiao,
                GhiChu = donHang.GhiChu,
                TongTienHang = donHang.TongTienHang,
                CodeKM = donHang.CodeKM,
                TienGiam = donHang.TienGiam,
                TongThanhToan = donHang.TongThanhToan,
                TrangThai = donHang.TrangThai,
                NgayVanDon = donHang.NgayVanDon,
                NgayGiaoThanhCong = donHang.NgayGiaoThanhCong,
                MaVanDon = donHang.MaVanDon,
                CoTheHuy = donHang.CoTheHuy,
                ThanhToan = donHang.ThanhToan == null ? null : new ThanhToanApiDto
                {
                    MaTT = donHang.ThanhToan.MaTT,
                    MaPT = donHang.ThanhToan.MaPT,
                    TenPT = donHang.ThanhToan.TenPT,
                    SoTien = donHang.ThanhToan.SoTien,
                    TrangThai = donHang.ThanhToan.TrangThai,
                    TrangThaiHoanTien = donHang.ThanhToan.TrangThaiHoanTien,
                    MaGiaoDich = donHang.ThanhToan.MaGiaoDich,
                    NgayThanhToan = donHang.ThanhToan.NgayThanhToan,
                    NgayHoanTien = donHang.ThanhToan.NgayHoanTien
                },
                ChiTiet = donHang.ChiTiet.Select(s => new ChiTietSanPhamDonHangDto
                {
                    MaCTDH = s.MaCTDH,
                    MaSP = s.MaSP,
                    TenSP = s.TenSP,
                    DuongDanAnh = s.DuongDanAnh,
                    DonGia = s.DonGia,
                    SoLuong = s.SoLuong,
                    ThanhTien = s.ThanhTien,
                    DaDanhGia = s.DaDanhGia,
                    TrangThaiDanhGia = s.TrangThaiDanhGia
                }).ToList(),
                LichSu = donHang.LichSu.Select(ls => new LichSuDonHangApiDto
                {
                    MaLS = ls.MaLS,
                    TrangThaiCu = ls.TrangThaiCu,
                    TrangThaiMoi = ls.TrangThaiMoi,
                    NgayDoi = ls.NgayDoi
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Hủy đơn hàng đang ở trạng thái Chờ xác nhận.
        /// </summary>
        /// <param name="maDH">Mã đơn hàng cần hủy</param>
        [HttpPost("{maDH:int}/huy")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(typeof(HuyDonHangResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HuyDon(int maDH)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue || maTK.Value <= 0)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = _localizer["Api_Error_UnauthorizedTitle"].Value,
                    Detail = _localizer["Api_Error_UnauthorizedDetail"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _donHangService.HuyDonAsync(maTK.Value, maDH);
            if (result.Success)
            {
                return Ok(new HuyDonHangResponseDto
                {
                    MaDH = maDH,
                    TrangThai = TrangThaiDonHangConstant.DaHuy,
                    ThongBao = _localizer["Order_CancelSuccess"].Value
                });
            }

            if (result.LoaiLoi == LoaiLoiDonHang.KhongTimThay)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Msg_Order_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (result.LoaiLoi == LoaiLoiDonHang.XungDotTrangThai)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Msg_Order_CannotCancel"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (result.LoaiLoi == LoaiLoiDonHang.LoiHeThong)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = _localizer["Api_Error_InternalServerErrorTitle"].Value,
                    Detail = _localizer["Msg_Order_CancelGeneralError"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = _localizer["Api_Error_BadRequestTitle"].Value,
                Detail = result.ErrorMessage ?? _localizer["Common_Error"].Value,
                Instance = HttpContext.Request.Path
            });
        }
    }
}
