using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/khuyen-mai")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class KhuyenMaiApiController : ControllerBase
    {
        private readonly IKhuyenMaiService _khuyenMaiService;
        private readonly IDatHangService _datHangService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhuyenMaiApiController(
            IKhuyenMaiService khuyenMaiService,
            IDatHangService datHangService,
            IStringLocalizer<SharedResource> localizer)
        {
            _khuyenMaiService = khuyenMaiService;
            _datHangService = datHangService;
            _localizer = localizer;
        }

        /// <summary>
        /// Lấy danh sách các mã khuyến mãi công khai đang diễn ra và còn lượt dùng
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<KhuyenMaiDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _khuyenMaiService.LayKhuyenMaiDangDienRaAsync();
            var dtos = list.Select(k => new KhuyenMaiDto
            {
                MaKM = k.MaKM,
                Code = k.Code,
                TenKM = k.TenKM,
                LoaiGiam = k.LoaiGiam,
                GiaTriGiam = k.GiaTriGiam,
                GiamToiDa = k.GiamToiDa,
                DonToiThieu = k.DonToiThieu,
                NgayBatDau = DateTime.MinValue,
                NgayKetThuc = k.NgayKetThuc,
                SoLuotToiDa = k.SoLuotToiDa,
                SoLuotDaDung = k.SoLuotDaDung
            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ và tính số tiền giảm của mã khuyến mãi cho các dòng giỏ được chọn hoặc nguồn Mua ngay
        /// </summary>
        [HttpPost("kiem-tra")]
        [Authorize(Roles = VaiTroConstant.KhachHang)]
        [ProducesResponseType(typeof(KiemTraKhuyenMaiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> KiemTraKhuyenMai([FromBody] KiemTraKhuyenMaiRequestDto request)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = _localizer["Api_Error_UnauthorizedTitle"].Value,
                    Detail = _localizer["Api_Error_UnauthorizedDetail"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Validation_Required"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var codeTrim = request.Code.Trim();
            if (codeTrim.Length > 30)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Validation_MaxLength", 30].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            bool hasCart = request.MaGHDaChon != null && request.MaGHDaChon.Any();
            bool hasBuyNow = request.MaSP.HasValue && request.MaSP.Value > 0 && request.SoLuong.HasValue && request.SoLuong.Value >= 1 && request.SoLuong.Value <= 99;

            if ((hasCart && hasBuyNow) || (!hasCart && !hasBuyNow))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Msg_Order_SourceInvalid"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            decimal tongTienHang = 0;

            if (hasCart)
            {
                var cartCheck = await _datHangService.KiemTraNguonGioHangChoKhuyenMaiAsync(maTK.Value, request.MaGHDaChon!);
                if (!cartCheck.HopLe)
                {
                    return StatusCode(cartCheck.StatusCode, new ProblemDetails
                    {
                        Status = cartCheck.StatusCode,
                        Title = cartCheck.ErrorTitle,
                        Detail = cartCheck.ErrorDetail,
                        Instance = HttpContext.Request.Path
                    });
                }
                tongTienHang = cartCheck.TongTienHang;
            }
            else
            {
                var buyNowCheck = await _datHangService.KiemTraNguonMuaNgayChoKhuyenMaiAsync(request.MaSP!.Value, request.SoLuong!.Value);
                if (!buyNowCheck.HopLe)
                {
                    return StatusCode(buyNowCheck.StatusCode, new ProblemDetails
                    {
                        Status = buyNowCheck.StatusCode,
                        Title = buyNowCheck.ErrorTitle,
                        Detail = buyNowCheck.ErrorDetail,
                        Instance = HttpContext.Request.Path
                    });
                }
                tongTienHang = buyNowCheck.TongTienHang;
            }

            var ketQua = await _khuyenMaiService.TinhGiamAsync(codeTrim, tongTienHang);

            var response = new KiemTraKhuyenMaiResponseDto
            {
                HopLe = ketQua.HopLe,
                ThongBao = ketQua.ThongBao ?? string.Empty,
                TienGiam = ketQua.TienGiam,
                TongTienHang = ketQua.TongTienHang,
                TongThanhToan = ketQua.TongThanhToan,
                Code = ketQua.Code,
                MaKM = ketQua.MaKM
            };

            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách các mã khuyến mãi khả dụng cho các dòng giỏ hàng được chọn
        /// </summary>
        [HttpGet("kha-dung")]
        [Authorize(Roles = VaiTroConstant.KhachHang)]
        [ProducesResponseType(typeof(List<KhuyenMaiDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LayDanhSachKhaDung([FromQuery] List<int>? maGH = null)
        {
            var maTK = User.LayMaTK();
            if (!maTK.HasValue)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = _localizer["Api_Error_UnauthorizedTitle"].Value,
                    Detail = _localizer["Api_Error_UnauthorizedDetail"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (maGH == null || maGH.Count == 0)
            {
                return Ok(new List<KhuyenMaiDto>());
            }

            var cartCheck = await _datHangService.KiemTraNguonGioHangChoKhuyenMaiAsync(maTK.Value, maGH);
            if (!cartCheck.HopLe)
            {
                return StatusCode(cartCheck.StatusCode, new ProblemDetails
                {
                    Status = cartCheck.StatusCode,
                    Title = cartCheck.ErrorTitle,
                    Detail = cartCheck.ErrorDetail,
                    Instance = HttpContext.Request.Path
                });
            }

            var list = await _khuyenMaiService.LayDanhSachMaKhaDungAsync(cartCheck.TongTienHang);
            var dtos = list.Select(k => new KhuyenMaiDto
            {
                MaKM = k.MaKM,
                Code = k.Code,
                TenKM = k.TenKM,
                LoaiGiam = k.LoaiGiam,
                GiaTriGiam = k.GiaTriGiam,
                GiamToiDa = k.GiamToiDa,
                DonToiThieu = k.DonToiThieu,
                NgayBatDau = DateTime.MinValue,
                NgayKetThuc = k.NgayKetThuc,
                SoLuotToiDa = k.SoLuotToiDa,
                SoLuotDaDung = k.SoLuotDaDung
            }).ToList();

            return Ok(dtos);
        }
    }
}
