using System.Security.Claims;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Lấy MaTK an toàn từ ClaimTypes.NameIdentifier
        /// </summary>
        public static int? LayMaTK(this ClaimsPrincipal? user)
        {
            if (user == null) return null;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var maTk))
            {
                return maTk;
            }

            return null;
        }

        /// <summary>
        /// Kiểm tra người dùng có vai trò Quản trị viên hay không
        /// </summary>
        public static bool LaAdmin(this ClaimsPrincipal? user)
        {
            if (user == null) return false;
            return user.IsInRole(VaiTroConstant.QuanTriVien);
        }
    }
}

