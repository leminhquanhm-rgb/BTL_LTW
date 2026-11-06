using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/thuong-hieu")]
    [AllowAnonymous]
    public class ThuongHieuApiController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public ThuongHieuApiController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        /// <summary>
        /// Lấy danh sách các thương hiệu mỹ phẩm đang hoạt động
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ThuongHieuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _sanPhamService.LayDanhSachThuongHieuAsync();
            var dtos = list.Select(t => new ThuongHieuDto
            {
                MaTH = t.MaTH,
                TenTH = t.TenTH,
                Logo = t.Logo,
                MoTa = t.MoTa
            }).ToList();

            return Ok(dtos);
        }
    }
}

