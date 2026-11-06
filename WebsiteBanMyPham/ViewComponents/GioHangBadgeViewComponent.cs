using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.ViewComponents
{
    public class GioHangBadgeViewComponent : ViewComponent
    {
        private readonly IGioHangService _gioHangService;

        public GioHangBadgeViewComponent(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Chỉ gọi service khi User đã đăng nhập VÀ có vai trò KhachHang
            if (UserClaimsPrincipal.Identity?.IsAuthenticated == true && UserClaimsPrincipal.IsInRole(VaiTroConstant.KhachHang))
            {
                var maTKStr = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(maTKStr, out int maTK))
                {
                    int soLuong = await _gioHangService.DemSoLuongAsync(maTK);
                    return View(soLuong);
                }
            }

            // Guest hoặc Admin: không gọi service, trả về null để không hiển thị badge số lượng
            return View((int?)null);
        }
    }
}

