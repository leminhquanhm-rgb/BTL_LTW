using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
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

            // Cấu hình OpenAPI (.NET 10) tài liệu hóa các endpoint Web API (loại bỏ MVC controller)
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Serene Cosmetics Web API",
                        Version = "v1",
                        Description = "Tài liệu RESTful Web API cho Website Quản lý và Kinh doanh Mỹ phẩm Trực tuyến Serene Cosmetics (.NET 10).\n\n" +
                                      "LƯU Ý BẢO MẬT: Mọi thao tác ghi (POST, PUT, DELETE) yêu cầu Cookie đăng nhập (xác thực phiên) và Header RequestVerificationToken (chống giả mạo CSRF)."
                    };

                    // Chỉ đưa các controller trong Controllers/Api/ vào tài liệu (loại MVC controller)
                    var nonApiPaths = document.Paths
                        .Where(p => !p.Key.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                        .Select(p => p.Key)
                        .ToList();

                    foreach (var path in nonApiPaths)
                    {
                        document.Paths.Remove(path);
                    }

                    return Task.CompletedTask;
                });
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

            // Đăng ký Dịch vụ chống đặt trùng (Singleton với máy trạng thái atomic)
            builder.Services.AddSingleton<IOrderAntiDuplicateService, OrderAntiDuplicateService>();

            // Đăng ký Services nghiệp vụ
            builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
            builder.Services.AddScoped<ISanPhamService, SanPhamService>();
            builder.Services.AddScoped<IGioHangService, GioHangService>();
            builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>();
            builder.Services.AddScoped<IDonHangService, DonHangService>();
            builder.Services.AddScoped<IDatHangService, DatHangService>();
            builder.Services.AddScoped<IDiaChiService, DiaChiService>();
            builder.Services.AddScoped<IDanhGiaService, DanhGiaService>();
            builder.Services.AddScoped<IKhieuNaiService, KhieuNaiService>();
            builder.Services.AddScoped<IDanhMucService, DanhMucService>();
            builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>();
            builder.Services.AddScoped<IThongKeService, ThongKeService>();
            builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            builder.Services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
            builder.Services.AddScoped<ISanPhamAdminService, SanPhamAdminService>();
            builder.Services.AddScoped<IThanhToanAdminService, ThanhToanAdminService>();
            builder.Services.AddScoped<IKhuyenMaiAdminService, KhuyenMaiAdminService>();
            builder.Services.AddScoped<IBannerAdminService, BannerAdminService>();
            builder.Services.AddScoped<ITaiKhoanAdminService, TaiKhoanAdminService>();
            builder.Services.AddScoped<IDanhGiaAdminService, DanhGiaAdminService>();
            builder.Services.AddScoped<IKhieuNaiAdminService, KhieuNaiAdminService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Bật tài liệu OpenAPI và giao diện Scalar chỉ trong môi trường Development
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Serene Cosmetics API Reference");
                    options.WithTheme(ScalarTheme.Moon);
                });
            }
            else
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
