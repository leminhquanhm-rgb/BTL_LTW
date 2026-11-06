using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteBanMyPham.Controllers
{
    public class CultureController : Controller
    {
        private static readonly HashSet<string> SupportedCultures = new(StringComparer.OrdinalIgnoreCase)
        {
            "vi-VN",
            "en-US"
        };

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(culture) && SupportedCultures.Contains(culture))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = Request.IsHttps,
                        HttpOnly = true
                    }
                );
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "TrangChu");
        }
    }
}

