using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly ILogger<TaiKhoanService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TaiKhoanService(
            WebsiteBanMyPhamContext context,
            ILogger<TaiKhoanService> logger,
            IWebHostEnvironment env,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _env = env;
            _localizer = localizer;
        }

        public async Task<(bool Success, string? ErrorMessage)> DangKyAsync(DangKyViewModel model)
        {
            var emailChuanHoa = model.Email.Trim().ToLowerInvariant();

            var daTonTai = await _context.TaiKhoan.AnyAsync(t => t.Email == emailChuanHoa);
            if (daTonTai)
            {
                return (false, _localizer["Msg_Account_EmailExists"].Value);
            }

            var matKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau, workFactor: 11);

            var taiKhoanMoi = new TaiKhoan
            {
                Email = emailChuanHoa,
                MatKhauHash = matKhauHash,
                HoTen = model.HoTen.Trim(),
                SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai) ? null : model.SoDienThoai.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim(),
                VaiTro = VaiTroConstant.KhachHang,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            try
            {
                _context.TaiKhoan.Add(taiKhoanMoi);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Lỗi cập nhật CSDL khi đăng ký tài khoản với email {Email}", emailChuanHoa);
                return (false, _localizer["Msg_Account_RegisterFailed"].Value);
            }
        }

        public async Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> DangNhapAsync(DangNhapViewModel model)
        {
            var emailChuanHoa = model.Email.Trim().ToLowerInvariant();

            var user = await _context.TaiKhoan.FirstOrDefaultAsync(t => t.Email == emailChuanHoa);
            if (user == null)
            {
                return (false, _localizer["Msg_Account_InvalidCredentials"].Value, null);
            }

            bool matKhauHopLe = false;
            try
            {
                matKhauHopLe = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhauHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi giải mã băm mật khẩu cho tài khoản {Email}", emailChuanHoa);
                return (false, _localizer["Msg_Account_InvalidCredentials"].Value, null);
            }

            if (!matKhauHopLe)
            {
                return (false, _localizer["Msg_Account_InvalidCredentials"].Value, null);
            }

            if (!user.TrangThai)
            {
                return (false, _localizer["Msg_Account_Locked", _localizer["Footer_ContactPhone"].Value].Value, null);
            }

            return (true, null, user);
        }

        public (ClaimsPrincipal Principal, AuthenticationProperties Properties) TaoClaimsPrincipal(TaiKhoan taiKhoan, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTK.ToString()),
                new Claim(ClaimTypes.Name, taiKhoan.HoTen),
                new Claim(ClaimTypes.Email, taiKhoan.Email),
                new Claim(ClaimTypes.Role, taiKhoan.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            if (rememberMe)
            {
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
            }

            return (principal, properties);
        }

        public async Task<(bool Success, string? ErrorMessage)> DoiMatKhauAsync(int maTk, DoiMatKhauViewModel model)
        {
            var user = await _context.TaiKhoan.FindAsync(maTk);
            if (user == null)
            {
                return (false, _localizer["Msg_Account_NotFound"].Value);
            }

            if (!user.TrangThai)
            {
                return (false, _localizer["Msg_Account_Locked", _localizer["Footer_ContactPhone"].Value].Value);
            }

            bool matKhauCuHopLe = false;
            try
            {
                matKhauCuHopLe = BCrypt.Net.BCrypt.Verify(model.MatKhauHienTai, user.MatKhauHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra mật khẩu hiện tại cho MaTK {MaTK}", maTk);
                return (false, _localizer["Msg_Account_WrongCurrentPassword"].Value);
            }

            if (!matKhauCuHopLe)
            {
                return (false, _localizer["Msg_Account_WrongCurrentPassword"].Value);
            }

            if (BCrypt.Net.BCrypt.Verify(model.MatKhauMoi, user.MatKhauHash))
            {
                return (false, _localizer["Msg_Account_SameOldPassword"].Value);
            }

            user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi, workFactor: 11);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Message)> TaoOtpQuenMatKhauAsync(string email)
        {
            var emailChuanHoa = email.Trim().ToLowerInvariant();
            var thongBaoChung = _localizer["Msg_Account_OtpSentGeneral"].Value;

            var user = await _context.TaiKhoan.FirstOrDefaultAsync(t => t.Email == emailChuanHoa);
            if (user == null || !user.TrangThai)
            {
                return (true, thongBaoChung);
            }

            // Vô hiệu hóa tất cả OTP cũ chưa dùng của cùng tài khoản
            var otpsCu = await _context.DatLaiMatKhau
                .Where(d => d.MaTK == user.MaTK && !d.DaDung)
                .ToListAsync();

            foreach (var otpCu in otpsCu)
            {
                otpCu.DaDung = true;
            }

            // Sinh mã OTP 6 chữ số ngẫu nhiên an toàn
            var otpValue = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var otpHash = BCrypt.Net.BCrypt.HashPassword(otpValue, workFactor: 10);
            var hetHan = DateTime.Now.AddMinutes(10);

            var banGhiOtp = new DatLaiMatKhau
            {
                MaTK = user.MaTK,
                OtpHash = otpHash,
                HetHan = hetHan,
                DaDung = false,
                SoLanSai = 0,
                NgayTao = DateTime.Now
            };

            _context.DatLaiMatKhau.Add(banGhiOtp);
            await _context.SaveChangesAsync();

            // Chỉ ghi OTP ra log khi môi trường là Development
            if (_env.IsDevelopment())
            {
                _logger.LogInformation("==================================================================");
                _logger.LogInformation("[DEMO OTP] Mã OTP đặt lại mật khẩu cho tài khoản {Email} là: {Otp}", user.Email, otpValue);
                _logger.LogInformation("[DEMO OTP] Mã có hiệu lực đến: {HetHan:HH:mm:ss dd/MM/yyyy}", hetHan);
                _logger.LogInformation("==================================================================");
            }

            return (true, thongBaoChung);
        }

        public async Task<(bool Success, string? ErrorMessage)> XacMinhOtpAsync(XacMinhOtpViewModel model)
        {
            var ketQua = await KiemTraOtpNoiBoAsync(model.Email, model.Otp);
            if (!ketQua.Success)
            {
                return (false, ketQua.ErrorMessage);
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DatLaiMatKhauAsync(DatLaiMatKhauViewModel model)
        {
            var ketQua = await KiemTraOtpNoiBoAsync(model.Email, model.Otp);
            if (!ketQua.Success || ketQua.DatLai == null || ketQua.User == null)
            {
                return (false, ketQua.ErrorMessage);
            }

            var user = ketQua.User;

            // Băm mật khẩu mới bằng BCrypt workFactor 11
            user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi, workFactor: 11);

            // Đánh dấu bản ghi OTP hiện tại là đã dùng
            ketQua.DatLai.DaDung = true;

            // Vô hiệu hóa mọi OTP còn lại của tài khoản này
            var cacOtpConLai = await _context.DatLaiMatKhau
                .Where(d => d.MaTK == user.MaTK && !d.DaDung)
                .ToListAsync();

            foreach (var item in cacOtpConLai)
            {
                item.DaDung = true;
            }

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<HoSoViewModel?> GetHoSoAsync(int maTk)
        {
            var user = await _context.TaiKhoan.FindAsync(maTk);
            if (user == null)
            {
                return null;
            }

            return new HoSoViewModel
            {
                MaTK = user.MaTK,
                Email = user.Email,
                HoTen = user.HoTen,
                SoDienThoai = user.SoDienThoai,
                DiaChi = user.DiaChi,
                VaiTro = user.VaiTro,
                NgayTao = user.NgayTao
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatHoSoAsync(int maTk, HoSoViewModel model)
        {
            var user = await _context.TaiKhoan.FindAsync(maTk);
            if (user == null)
            {
                return (false, _localizer["Msg_Account_NotFound"].Value);
            }

            if (!user.TrangThai)
            {
                return (false, _localizer["Msg_Account_Locked", _localizer["Footer_ContactPhone"].Value].Value);
            }

            // Chỉ cho phép cập nhật HoTen, SoDienThoai, DiaChi. Không cho đổi Email và VaiTro.
            user.HoTen = model.HoTen.Trim();
            user.SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai) ? null : model.SoDienThoai.Trim();
            user.DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim();

            await _context.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Hàm nội bộ dùng chung để kiểm tra và xác thực OTP, bảo vệ chống brute-force và lạm dụng qua cả bước xác minh lẫn đặt lại mật khẩu.
        /// </summary>
        private async Task<(bool Success, string? ErrorMessage, DatLaiMatKhau? DatLai, TaiKhoan? User)> KiemTraOtpNoiBoAsync(string email, string otp)
        {
            var emailChuanHoa = email.Trim().ToLowerInvariant();

            var user = await _context.TaiKhoan.FirstOrDefaultAsync(t => t.Email == emailChuanHoa);
            if (user == null || !user.TrangThai)
            {
                // Khi tài khoản không tồn tại hoặc bị khóa, trả cùng thông báo chung để bảo mật
                return (false, _localizer["Msg_Account_OtpInvalidOrExpired"].Value, null, null);
            }

            // Tìm bản ghi OTP mới nhất chưa dùng của tài khoản
            var datLai = await _context.DatLaiMatKhau
                .Where(d => d.MaTK == user.MaTK && !d.DaDung)
                .OrderByDescending(d => d.MaDL)
                .FirstOrDefaultAsync();

            if (datLai == null)
            {
                return (false, _localizer["Msg_Account_OtpInvalidOrExpired"].Value, null, null);
            }

            if (datLai.HetHan < DateTime.Now)
            {
                return (false, _localizer["Msg_Account_OtpExpiredNeedNew"].Value, null, null);
            }

            // Tăng SoLanSai bằng câu UPDATE nguyên tử để chống vượt giới hạn khi gửi nhiều request song song
            var affected = await _context.DatLaiMatKhau
                .Where(d => d.MaDL == datLai.MaDL && !d.DaDung && d.SoLanSai < 5)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.SoLanSai, d => d.SoLanSai + 1));

            if (affected == 0)
            {
                return (false, _localizer["Msg_Account_OtpLocked5Times"].Value, null, null);
            }

            datLai.SoLanSai += 1;

            bool otpDung = false;
            try
            {
                otpDung = BCrypt.Net.BCrypt.Verify(otp.Trim(), datLai.OtpHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra băm OTP cho tài khoản {Email}", emailChuanHoa);
                return (false, _localizer["Msg_Account_OtpInvalidOrExpired"].Value, null, null);
            }

            if (otpDung)
            {
                // Nếu mã đúng: giảm lại 1 lần sai để bước xác minh không tiêu hao lượt của bước đặt lại mật khẩu
                await _context.DatLaiMatKhau
                    .Where(d => d.MaDL == datLai.MaDL && !d.DaDung && d.SoLanSai > 0)
                    .ExecuteUpdateAsync(s => s.SetProperty(d => d.SoLanSai, d => d.SoLanSai - 1));

                datLai.SoLanSai -= 1;
                return (true, null, datLai, user);
            }

            // Nếu mã sai: số lần còn lại tính theo giá trị sau khi tăng
            if (datLai.SoLanSai >= 5)
            {
                return (false, _localizer["Msg_Account_OtpLocked5Times"].Value, null, null);
            }

            int soLanConLai = 5 - datLai.SoLanSai;
            return (false, _localizer["Msg_Account_OtpIncorrectAttemptsLeft", soLanConLai].Value, null, null);
        }
    }
}
