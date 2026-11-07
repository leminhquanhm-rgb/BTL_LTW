using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Helpers;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình MemoryCache cho ứng dụng (dùng cho xác thực Cookie và tra cứu nhanh)
            builder.Services.AddMemoryCache();

            // Cấu hình Đa ngôn ngữ (Localization)
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews(options =>
            {
                // Bộ lọc tự động chống tấn công CSRF cho tất cả action thay đổi dữ liệu (POST, PUT, DELETE)
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                });

            builder.Services.AddDbContext<WebsiteBanMyPhamContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cấu hình Anti-Forgery Token cho Header (gọi qua AJAX / apiFetch)
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "RequestVerificationToken";
            });

            // Cấu hình Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/TaiKhoan/DangNhap";
                    options.AccessDeniedPath = "/TaiKhoan/TuChoiTruyCap";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = CookieValidator.ValidatePrincipalAsync,
                        OnRedirectToLogin = async context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api"))
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                context.Response.ContentType = "application/problem+json";
                                var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                                var problem = new ProblemDetails
                                {
                                    Status = StatusCodes.Status401Unauthorized,
                                    Title = localizer["Api_Error_UnauthorizedTitle"].Value,
                                    Detail = localizer["Api_Error_UnauthorizedDetail"].Value,
                                    Instance = context.Request.Path
                                };
                                await context.Response.WriteAsJsonAsync(problem);
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                        },
                        OnRedirectToAccessDenied = async context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api"))
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                context.Response.ContentType = "application/problem+json";
                                var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                                var problem = new ProblemDetails
                                {
                                    Status = StatusCodes.Status403Forbidden,
                                    Title = localizer["Api_Error_ForbiddenTitle"].Value,
                                    Detail = localizer["Api_Error_ForbiddenDetail"].Value,
                                    Instance = context.Request.Path
                                };
                                await context.Response.WriteAsJsonAsync(problem);
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                        }
                    };
                });

            // Đăng ký Helper đa ngôn ngữ
            builder.Services.AddScoped<ILocalizationHelper, LocalizationHelper>();

            // Đăng ký Services nghiệp vụ
            builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
            builder.Services.AddScoped<ISanPhamService, SanPhamService>();
            builder.Services.AddScoped<IGioHangService, GioHangService>();
            builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>();
            builder.Services.AddScoped<IDonHangService, DonHangService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/TrangChu/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Cấu hình Request Localization trong pipeline (ngay sau UseRouting và trước UseAuthentication)
            var supportedCultures = new[]
            {
                new CultureInfo("vi-VN"),
                new CultureInfo("en-US")
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("vi-VN"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            // Chỉ dùng CookieRequestCultureProvider (không dùng Accept-Language để tự đổi ngôn ngữ)
            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

            app.UseRequestLocalization(localizationOptions);

            // Authentication phải đặt trước Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=TrangChu}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
