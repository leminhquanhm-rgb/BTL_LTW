using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api.Admin
{
    /// <summary>
    /// API Quản trị Thương hiệu sản phẩm (Chỉ dành cho Quản trị viên)
    /// </summary>
    [ApiController]
    [Route("api/v1/admin/thuong-hieu")]
    [Authorize(Roles = VaiTroConstant.QuanTriVien)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ThuongHieuAdminApiController : ControllerBase
    {
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ThuongHieuAdminApiController(
            IThuongHieuService thuongHieuService,
            IStringLocalizer<SharedResource> localizer)
        {
            _thuongHieuService = thuongHieuService;
            _localizer = localizer;
        }

        /// <summary>
        /// Lấy danh sách thương hiệu có tìm kiếm, lọc trạng thái và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm theo tên thương hiệu</param>
        /// <param name="trangThai">Lọc theo trạng thái hoạt động (true/false)</param>
        /// <param name="pageIndex">Trang hiện tại (>= 1, mặc định 1)</param>
        /// <param name="pageSize">Số dòng mỗi trang (1 - 50, mặc định 10)</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<ThuongHieuAdminDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSach(
            [FromQuery] string? keyword = null,
            [FromQuery] bool? trangThai = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _thuongHieuService.LayDanhSachAdminAsync(keyword, trangThai, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thông tin một thương hiệu theo mã ID
        /// </summary>
        /// <param name="id">Mã thương hiệu (MaTH)</param>
        [HttpGet("{id:int}")]
        [ActionName(nameof(GetChiTiet))]
        [ProducesResponseType(typeof(ThuongHieuAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            var item = await _thuongHieuService.LayChiTietAdminAsync(id);
            if (item == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(item);
        }

        /// <summary>
        /// Thêm mới một thương hiệu mỹ phẩm
        /// </summary>
        /// <param name="request">Thông tin thương hiệu mới</param>
        [HttpPost]
        [ProducesResponseType(typeof(ThuongHieuAdminDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Tao([FromBody] TaoThuongHieuRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (loaiLoi, maTH, dto) = await _thuongHieuService.TaoAsync(request.TenTH, request.MoTa, request.TrangThai);

            if (loaiLoi == LoaiLoiThuongHieu.TrungTen)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_DuplicateName", request.TenTH.Trim()].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi != LoaiLoiThuongHieu.ThanhCong || dto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_BadRequestTitle"].Value,
                    Detail = _localizer["Common_Error"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return CreatedAtAction(nameof(GetChiTiet), new { id = maTH }, dto);
        }

        /// <summary>
        /// Cập nhật toàn phần một thương hiệu theo mã ID (giữ nguyên Logo hiện tại)
        /// </summary>
        /// <param name="id">Mã thương hiệu cần sửa (MaTH)</param>
        /// <param name="request">Thông tin cập nhật</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ThuongHieuAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CapNhat(int id, [FromBody] CapNhatThuongHieuRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (loaiLoi, dto) = await _thuongHieuService.CapNhatAsync(id, request.TenTH, request.MoTa, request.TrangThai);

            if (loaiLoi == LoaiLoiThuongHieu.KhongTimThay)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi == LoaiLoiThuongHieu.TrungTen)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_DuplicateName", request.TenTH.Trim()].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi != LoaiLoiThuongHieu.ThanhCong || dto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_BadRequestTitle"].Value,
                    Detail = _localizer["Common_Error"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(dto);
        }

        /// <summary>
        /// Xóa một thương hiệu khi chưa có sản phẩm nào liên kết
        /// </summary>
        /// <param name="id">Mã thương hiệu cần xóa</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Xoa(int id)
        {
            var (loaiLoi, soLuongSP) = await _thuongHieuService.XoaAsync(id);

            if (loaiLoi == LoaiLoiThuongHieu.KhongTimThay)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi == LoaiLoiThuongHieu.DangCoSanPhamThamChieu)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Brand_HasProducts", soLuongSP].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }
    }
}

