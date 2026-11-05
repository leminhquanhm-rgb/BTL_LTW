using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public interface ITaiKhoanService
    {
        Task<(bool Success, string? ErrorMessage)> DangKyAsync(DangKyViewModel model);

        Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> DangNhapAsync(DangNhapViewModel model);

        (ClaimsPrincipal Principal, AuthenticationProperties Properties) TaoClaimsPrincipal(TaiKhoan taiKhoan, bool rememberMe);

        Task<(bool Success, string? ErrorMessage)> DoiMatKhauAsync(int maTk, DoiMatKhauViewModel model);

        Task<(bool Success, string? Message)> TaoOtpQuenMatKhauAsync(string email);

        Task<(bool Success, string? ErrorMessage)> XacMinhOtpAsync(XacMinhOtpViewModel model);

        Task<(bool Success, string? ErrorMessage)> DatLaiMatKhauAsync(DatLaiMatKhauViewModel model);

        Task<HoSoViewModel?> GetHoSoAsync(int maTk);

        Task<(bool Success, string? ErrorMessage)> CapNhatHoSoAsync(int maTk, HoSoViewModel model);
    }
}

