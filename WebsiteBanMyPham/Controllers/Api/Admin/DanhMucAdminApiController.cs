using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api.Admin
{
    /// <summary>
    /// API Quản trị Danh mục sản phẩm (Chỉ dành cho Quản trị viên)
    /// </summary>
    [ApiController]
    [Route("api/v1/admin/danh-muc")]
    [Authorize(Roles = VaiTroConstant.QuanTriVien)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DanhMucAdminApiController : ControllerBase
    {
        private readonly IDanhMucService _danhMucService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DanhMucAdminApiController(
            IDanhMucService danhMucService,
            IStringLocalizer<SharedResource> localizer)
        {
            _danhMucService = danhMucService;
            _localizer = localizer;
        }

        /// <summary>
        /// Lấy danh sách danh mục có tìm kiếm, lọc trạng thái và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm theo tên danh mục</param>
        /// <param name="trangThai">Lọc theo trạng thái hoạt động (true/false)</param>
        /// <param name="pageIndex">Trang hiện tại (>= 1, mặc định 1)</param>
        /// <param name="pageSize">Số dòng mỗi trang (1 - 50, mặc định 10)</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<DanhMucAdminDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSach(
            [FromQuery] string? keyword = null,
            [FromQuery] bool? trangThai = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _danhMucService.LayDanhSachAdminAsync(keyword, trangThai, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thông tin một danh mục theo mã ID
        /// </summary>
        /// <param name="id">Mã danh mục (MaDM)</param>
        [HttpGet("{id:int}")]
        [ActionName(nameof(GetChiTiet))]
        [ProducesResponseType(typeof(DanhMucAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            var item = await _danhMucService.LayChiTietAdminAsync(id);
            if (item == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(item);
        }

        /// <summary>
        /// Thêm mới một danh mục mỹ phẩm
        /// </summary>
        /// <param name="request">Thông tin danh mục mới</param>
        [HttpPost]
        [ProducesResponseType(typeof(DanhMucAdminDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Tao([FromBody] TaoDanhMucRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (loaiLoi, maDM, dto) = await _danhMucService.TaoAsync(request.TenDM, request.MoTa, request.TrangThai);

            if (loaiLoi == LoaiLoiDanhMuc.TrungTen)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_DuplicateName", request.TenDM.Trim()].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi != LoaiLoiDanhMuc.ThanhCong || dto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["Api_Error_BadRequestTitle"].Value,
                    Detail = _localizer["Common_Error"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return CreatedAtAction(nameof(GetChiTiet), new { id = maDM }, dto);
        }

        /// <summary>
        /// Cập nhật toàn phần một danh mục theo mã ID
        /// </summary>
        /// <param name="id">Mã danh mục cần sửa (MaDM)</param>
        /// <param name="request">Thông tin cập nhật</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DanhMucAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CapNhat(int id, [FromBody] CapNhatDanhMucRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (loaiLoi, dto) = await _danhMucService.CapNhatAsync(id, request.TenDM, request.MoTa, request.TrangThai);

            if (loaiLoi == LoaiLoiDanhMuc.KhongTimThay)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi == LoaiLoiDanhMuc.TrungTen)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_DuplicateName", request.TenDM.Trim()].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi != LoaiLoiDanhMuc.ThanhCong || dto == null)
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
        /// Xóa một danh mục khi chưa có sản phẩm nào liên kết
        /// </summary>
        /// <param name="id">Mã danh mục cần xóa</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Xoa(int id)
        {
            var (loaiLoi, soLuongSP) = await _danhMucService.XoaAsync(id);

            if (loaiLoi == LoaiLoiDanhMuc.KhongTimThay)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = _localizer["Api_Error_NotFoundTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_NotFound"].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            if (loaiLoi == LoaiLoiDanhMuc.DangCoSanPhamThamChieu)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = _localizer["Api_Error_ConflictTitle"].Value,
                    Detail = _localizer["Api_Admin_Category_HasProducts", soLuongSP].Value,
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }
    }
}

