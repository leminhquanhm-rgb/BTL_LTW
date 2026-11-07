using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
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
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public KhuyenMaiApiController(
            IKhuyenMaiService khuyenMaiService,
            WebsiteBanMyPhamContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _khuyenMaiService = khuyenMaiService;
            _context = context;
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
        /// Kiểm tra tính hợp lệ và tính số tiền giảm của mã khuyến mãi cho các dòng giỏ được chọn
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

            if (request == null || string.IsNullOrWhiteSpace(request.Code) || request.MaGHDaChon == null)
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

            var distinctIds = request.MaGHDaChon.Distinct().ToList();
            if (distinctIds.Count == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Api_Error_EmptyCartSelection"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (distinctIds.Count > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Validation_MaxLength", 100].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            // Kiểm tra quyền sở hữu tất cả các dòng giỏ được chọn
            var items = await _context.GioHang
                .AsNoTracking()
                .Where(g => g.MaTK == maTK.Value && distinctIds.Contains(g.MaGH))
                .Include(g => g.MaSPNavigation)
                .ToListAsync();

            if (items.Count != distinctIds.Count)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Error_CartItemNotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            // Tự động tính tổng tiền hàng từ các dòng giỏ hợp lệ thuộc khách hàng
            var hopLeItems = items.Where(i => i.MaSPNavigation.TrangThai && i.MaSPNavigation.SoLuongTon >= i.SoLuong && i.SoLuong > 0).ToList();
            decimal tongTienHang = hopLeItems.Sum(i => i.MaSPNavigation.Gia * i.SoLuong);

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

            if (maGH == null || maGH.Count == 0)
            {
                return Ok(new List<KhuyenMaiDto>());
            }

            var distinctIds = maGH.Distinct().ToList();
            if (distinctIds.Count > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_InvalidPromoRequest"].Value,
                    Detail = _localizer["Validation_MaxLength", 100].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var items = await _context.GioHang
                .AsNoTracking()
                .Where(g => g.MaTK == maTK.Value && distinctIds.Contains(g.MaGH))
                .Include(g => g.MaSPNavigation)
                .ToListAsync();

            if (items.Count != distinctIds.Count)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Error_CartItemNotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            var hopLeItems = items.Where(i => i.MaSPNavigation.TrangThai && i.MaSPNavigation.SoLuongTon >= i.SoLuong && i.SoLuong > 0).ToList();
            decimal tongTienHang = hopLeItems.Sum(i => i.MaSPNavigation.Gia * i.SoLuong);

            var list = await _khuyenMaiService.LayDanhSachMaKhaDungAsync(tongTienHang);
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
