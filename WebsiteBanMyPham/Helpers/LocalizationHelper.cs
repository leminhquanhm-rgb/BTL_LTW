using System.Globalization;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham;

namespace WebsiteBanMyPham.Helpers
{
    public class LocalizationHelper : ILocalizationHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
        private static readonly CultureInfo EnCulture = CultureInfo.GetCultureInfo("en-US");

        public LocalizationHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string FormatCurrency(decimal? amount)
        {
            if (!amount.HasValue)
            {
                return string.Empty;
            }

            var currentCulture = CultureInfo.CurrentUICulture.Name;
            if (currentCulture.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                return $"{amount.Value.ToString("N0", EnCulture)} VND";
            }

            return $"{amount.Value.ToString("N0", ViCulture)}đ";
        }

        public string FormatDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }

            var currentCulture = CultureInfo.CurrentUICulture.Name;
            if (currentCulture.StartsWith("en", StringIgnoreCase))
            {
                return dateTime.Value.ToString("MM/dd/yyyy h:mm tt", EnCulture);
            }

            return dateTime.Value.ToString("dd/MM/yyyy HH:mm", ViCulture);
        }

        private static readonly StringComparison StringIgnoreCase = StringComparison.OrdinalIgnoreCase;

        public string GetOrderStatusLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            var localized = _localizer[$"Status_Order_{status}"];
            return localized.ResourceNotFound ? status : localized.Value;
        }

        public string GetPaymentStatusLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            var localized = _localizer[$"Status_Payment_{status}"];
            return localized.ResourceNotFound ? status : localized.Value;
        }

        public string GetRefundStatusLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            var localized = _localizer[$"Status_Refund_{status}"];
            return localized.ResourceNotFound ? status : localized.Value;
        }

        public string TenDanhMuc(int maDM, string tenGoc)
        {
            var localized = _localizer[$"DanhMuc_{maDM}"];
            if (localized.ResourceNotFound || string.IsNullOrWhiteSpace(localized.Value))
            {
                return tenGoc;
            }
            return localized.Value;
        }

        public string MaDonHang(int maDH)
        {
            return $"#DH{maDH}";
        }

        public string GetPaymentMethodLabel(string tenPT)
        {
            if (string.IsNullOrWhiteSpace(tenPT))
            {
                return string.Empty;
            }

            var localized = _localizer[$"Payment_Method_{tenPT}"];
            return localized.ResourceNotFound ? tenPT : localized.Value;
        }
    }
}
