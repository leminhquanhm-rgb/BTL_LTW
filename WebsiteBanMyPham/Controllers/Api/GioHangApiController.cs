using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/gio-hang")]
    [Authorize(Roles = VaiTroConstant.KhachHang)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class GioHangApiController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GioHangApiController(
            IGioHangService gioHangService,
            IStringLocalizer<SharedResource> localizer)
        {
            _gioHangService = gioHangService;
            _localizer = localizer;
        }

        private int? LayMaTaiKhoanHienTai()
        {
            var maTKStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(maTKStr, out int maTK))
            {
                return maTK;
            }
            return null;
        }

        /// <summary>
        /// Lấy toàn bộ các dòng sản phẩm trong giỏ hàng của khách hàng hiện tại
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(GioHangDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGioHang()
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            var vm = await _gioHangService.LayGioHangAsync(maTK.Value);
            var dto = new GioHangDto
            {
                TongSoLuong = vm.TongSoLuong,
                TongTien = vm.TongTien,
                Items = vm.Items.Select(i => new GioHangItemDto
                {
                    MaGH = i.MaGH,
                    MaSP = i.MaSP,
                    TenSP = i.TenSP,
                    DuongDanAnh = i.DuongDanAnh,
                    GiaHienTai = i.GiaHienTai,
                    GiaGoc = i.GiaGoc,
                    SoLuong = i.SoLuong,
                    ThanhTien = i.ThanhTien,
                    SoLuongTon = i.SoLuongTon,
                    ConHang = i.ConHang,
                    VuotTonKho = i.VuotTonKho,
                    HopLeDeDat = i.HopLeDeDat,
                    ThongBaoTrangThai = i.ThongBaoTrangThai
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Lấy tổng số lượng sản phẩm trong giỏ hàng để cập nhật badge header
        /// </summary>
        [HttpGet("so-luong")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSoLuong()
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            int soLuong = await _gioHangService.DemSoLuongAsync(maTK.Value);
            return Ok(new { tongSoLuong = soLuong });
        }

        /// <summary>
        /// Lấy thông tin một dòng giỏ hàng cụ thể theo mã dòng giỏ
        /// </summary>
        [HttpGet("items/{maGH}")]
        [ActionName(nameof(GetItem))]
        [ProducesResponseType(typeof(GioHangItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(int maGH)
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            var item = await _gioHangService.LayDongGioHangAsync(maTK.Value, maGH);
            if (item == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Error_CartItemNotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var dto = new GioHangItemDto
            {
                MaGH = item.MaGH,
                MaSP = item.MaSP,
                TenSP = item.TenSP,
                DuongDanAnh = item.DuongDanAnh,
                GiaHienTai = item.GiaHienTai,
                GiaGoc = item.GiaGoc,
                SoLuong = item.SoLuong,
                ThanhTien = item.ThanhTien,
                SoLuongTon = item.SoLuongTon,
                ConHang = item.ConHang,
                VuotTonKho = item.VuotTonKho,
                HopLeDeDat = item.HopLeDeDat,
                ThongBaoTrangThai = item.ThongBaoTrangThai
            };

            return Ok(dto);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng hoặc cộng dồn nếu sản phẩm đã có trong giỏ
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(typeof(GioHangKetQuaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GioHangKetQuaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ThemVaoGio([FromBody] ThemGioHangRequestDto request)
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Detail = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _gioHangService.ThemVaoGioAsync(maTK.Value, request.MaSP, request.SoLuong);

            if (!result.Success)
            {
                if (result.LoaiLoi == LoaiLoiGioHang.KhongTimThay)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = _localizer["Api_Error_NotFoundTitle"].Value,
                        Detail = result.ErrorMessage ?? _localizer["Api_Error_NotFoundTitle"].Value,
                        Instance = HttpContext.Request.Path
                    });
                }

                if (result.LoaiLoi == LoaiLoiGioHang.XungDotTonKhoHoacNgungBan)
                {
                    return Conflict(new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = _localizer["Cart_ItemOutOfStockWarning"].Value,
                        Detail = result.ErrorMessage ?? _localizer["Cart_ItemOutOfStockWarning"].Value,
                        Instance = HttpContext.Request.Path
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Detail = result.ErrorMessage ?? _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            int tongSoLuong = await _gioHangService.DemSoLuongAsync(maTK.Value);
            var responseDto = new GioHangKetQuaDto
            {
                MaGH = result.MaGH,
                SoLuong = result.SoLuongDong,
                TongSoLuongTrongGio = tongSoLuong
            };

            // Nếu đã tồn tại trước đó thì trả 200 OK; nếu tạo mới trả 201 Created kèm CreatedAtAction
            if (result.DaTonTaiTruocDo)
            {
                return Ok(responseDto);
            }

            return CreatedAtAction(nameof(GetItem), new { maGH = result.MaGH }, responseDto);
        }

        /// <summary>
        /// Cập nhật số lượng của một dòng giỏ hàng
        /// </summary>
        [HttpPut("items/{maGH}")]
        [ProducesResponseType(typeof(GioHangKetQuaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CapNhatSoLuong(int maGH, [FromBody] CapNhatGioHangRequestDto request)
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Detail = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _gioHangService.CapNhatSoLuongAsync(maTK.Value, maGH, request.SoLuong);

            if (!result.Success)
            {
                if (result.LoaiLoi == LoaiLoiGioHang.KhongTimThay)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = _localizer["Api_Error_NotFoundTitle"].Value,
                        Detail = result.ErrorMessage ?? _localizer["Api_Error_CartItemNotFound"].Value,
                        Instance = HttpContext.Request.Path
                    });
                }

                if (result.LoaiLoi == LoaiLoiGioHang.XungDotTonKhoHoacNgungBan)
                {
                    return Conflict(new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = _localizer["Cart_ItemExceedWarning"].Value,
                        Detail = result.ErrorMessage ?? _localizer["Cart_ItemExceedWarning"].Value,
                        Instance = HttpContext.Request.Path
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Detail = result.ErrorMessage ?? _localizer["Api_Error_CartQuantityInvalid"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            int tongSoLuong = await _gioHangService.DemSoLuongAsync(maTK.Value);
            return Ok(new GioHangKetQuaDto
            {
                MaGH = maGH,
                SoLuong = request.SoLuong,
                TongSoLuongTrongGio = tongSoLuong
            });
        }

        /// <summary>
        /// Xóa một dòng khỏi giỏ hàng
        /// </summary>
        [HttpDelete("items/{maGH}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> XoaDongGioHang(int maGH)
        {
            var maTK = LayMaTaiKhoanHienTai();
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

            var result = await _gioHangService.XoaKhoiGioAsync(maTK.Value, maGH);
            if (!result.Success)
            {
                if (result.LoaiLoi == LoaiLoiGioHang.KhongTimThay)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = _localizer["Api_Error_NotFoundTitle"].Value,
                        Detail = result.ErrorMessage ?? _localizer["Api_Error_CartItemNotFound"].Value,
                        Instance = HttpContext.Request.Path
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Msg_Cart_InvalidAction"].Value,
                    Detail = result.ErrorMessage ?? _localizer["Msg_Cart_InvalidAction"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }
    }
}

