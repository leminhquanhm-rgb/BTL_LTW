using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/danh-muc")]
    [AllowAnonymous]
    public class DanhMucApiController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public DanhMucApiController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        /// <summary>
        /// Lấy danh sách các danh mục mỹ phẩm đang hoạt động
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<DanhMucDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _sanPhamService.LayDanhSachDanhMucAsync();
            var dtos = list.Select(d => new DanhMucDto
            {
                MaDM = d.MaDM,
                TenDM = d.TenDM,
                MoTa = d.MoTa
            }).ToList();

            return Ok(dtos);
        }
    }
}

