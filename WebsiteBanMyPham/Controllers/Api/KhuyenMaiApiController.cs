using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Services;
using WebsiteBanMyPham.ViewModels.Api;

namespace WebsiteBanMyPham.Controllers.Api
{
    [ApiController]
    [Route("api/v1/khuyen-mai")]
    [AllowAnonymous]
    public class KhuyenMaiApiController : ControllerBase
    {
        private readonly IKhuyenMaiService _khuyenMaiService;

        public KhuyenMaiApiController(IKhuyenMaiService khuyenMaiService)
        {
            _khuyenMaiService = khuyenMaiService;
        }

        /// <summary>
        /// Lấy danh sách các mã khuyến mãi đang trong thời hạn và còn lượt sử dụng
        /// </summary>
        [HttpGet]
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
    }
}

