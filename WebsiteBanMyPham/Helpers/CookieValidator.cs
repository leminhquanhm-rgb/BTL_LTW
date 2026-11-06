using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebsiteBanMyPham.Models;

namespace WebsiteBanMyPham.Helpers
{
    public static class CookieValidator
    {
        private const string CacheKeyPrefix = "TaiKhoan_AuthValidation_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Kiểu dữ liệu lưu đệm trạng thái xác thực của tài khoản trong MemoryCache (30s)
        /// </summary>
        public record TaiKhoanAuthCache(bool Exists, bool TrangThai, string VaiTro);

        /// <summary>
        /// Xóa cache trạng thái tài khoản theo MaTK.
        /// Chú ý: Dùng tại Bước 6 khi Quản trị viên thực hiện Khóa / Mở khóa tài khoản hoặc thay đổi vai trò
        /// để người dùng bị áp dụng hiệu lực ngay lập tức mà không cần đợi hết hạn cache 30 giây.
        /// </summary>
        public static void XoaCacheTaiKhoan(IMemoryCache cache, int maTk)
        {
            if (cache != null)
            {
                cache.Remove(CacheKeyPrefix + maTk);
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của CookiePrincipal theo chu kỳ request.
        /// Sử dụng IMemoryCache (30s) để giảm tải truy vấn cơ sở dữ liệu.
        /// Lấy DbContext và MemoryCache trực tiếp từ HttpContext.RequestServices.
        /// </summary>
        public static async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;
            if (userPrincipal == null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            var idClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var maTk))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var cacheKey = CacheKeyPrefix + maTk;

            if (!cache.TryGetValue(cacheKey, out TaiKhoanAuthCache? authInfo))
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<WebsiteBanMyPhamContext>();

                var tk = await dbContext.TaiKhoan.AsNoTracking()
                    .Where(t => t.MaTK == maTk)
                    .Select(t => new { t.TrangThai, t.VaiTro })
                    .FirstOrDefaultAsync();

                if (tk == null)
                {
                    authInfo = new TaiKhoanAuthCache(false, false, string.Empty);
                }
                else
                {
                    authInfo = new TaiKhoanAuthCache(true, tk.TrangThai, tk.VaiTro);
                }

                cache.Set(cacheKey, authInfo, CacheDuration);
            }

            var roleClaim = userPrincipal.FindFirst(ClaimTypes.Role)?.Value;

            // Nếu tài khoản không tồn tại trong DB, bị khóa (TrangThai = false) hoặc vai trò đã bị thay đổi
            if (authInfo == null || !authInfo.Exists || !authInfo.TrangThai || authInfo.VaiTro != roleClaim)
            {
                // Xóa cache ngay khi phát hiện vi phạm
                cache.Remove(cacheKey);

                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}

